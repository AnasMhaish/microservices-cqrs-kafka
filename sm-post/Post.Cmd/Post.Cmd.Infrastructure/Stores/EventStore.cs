using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public EventStore(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
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

                await _eventStoreRepository.SaveAsync(eventModel);
            }
        }
    }
}
