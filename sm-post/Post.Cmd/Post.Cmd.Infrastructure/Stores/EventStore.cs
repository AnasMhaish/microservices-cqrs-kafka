using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        private readonly IMongoClient _mongoClient;

        public EventStore(
            IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer,
            IOptions<MongoDbConfig> mongoDbConfig
        )
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
            _mongoClient = new MongoClient(mongoDbConfig.Value.ConnectionString);
        }

        public async Task<IList<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdAsync(aggregateId);

            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException($"Aggregate not found: {aggregateId}");
            }

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(
            Guid aggregateId,
            IEnumerable<BaseEvent> events,
            int expectedVersion
        )
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdAsync(aggregateId);

            if (expectedVersion != -1 && eventStream.Last().Version != expectedVersion)
            {
                throw new ConcurrencyException($"Conflict detected for aggregate: {aggregateId}");
            }

            var version = expectedVersion;
            var eventModels = new List<EventModel>();

            foreach (var @event in events)
            {
                version++;
                var eventModel = new EventModel
                {
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    EventType = @event.GetType().Name,
                    EventData = @event,
                    TimeStamp = DateTime.UtcNow,
                    Version = version
                };

                eventModels.Add(eventModel);
            }

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

            try
            {
                foreach (var eventModel in eventModels)
                {
                    await _eventStoreRepository.SaveAsync(eventModel);
                    await _eventProducer.ProduceAsync(topic, eventModel.EventData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save events to MongoDB", ex);
            }
        }

        public async Task SaveEventWithTransactionAsync(List<EventModel> eventModels)
        {
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    foreach (var eventModel in eventModels)
                    {
                        await _eventStoreRepository.SaveAsync(eventModel);
                        await _eventProducer.ProduceAsync(topic, eventModel.EventData);
                    }

                    await session.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await session.AbortTransactionAsync();
                    throw new Exception("Failed to save events to MongoDB", ex);
                }
            }
        }
    }
}
