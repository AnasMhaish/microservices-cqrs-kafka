using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<PostEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<IList<PostEntity>>>> _handlers =
            new();

        public void RegisterHandler<TQuery>(Func<TQuery, Task<IList<PostEntity>>> handler)
            where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
                throw new IndexOutOfRangeException(
                    $"Handler for query {typeof(TQuery).Name} is already registered."
                );

            _handlers.Add(typeof(TQuery), query => handler((TQuery)query));
        }

        public async Task<IList<PostEntity>> SendAsync(BaseQuery query)
        {
            if (_handlers.TryGetValue(query.GetType(), out var handler))
                return await handler(query);

            throw new ArgumentNullException(
                $"Handler for query {query.GetType().Name} is not registered."
            );
        }
    }
}
