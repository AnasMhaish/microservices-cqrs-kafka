using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Queries;

namespace CQRS.Core.Infrastructure
{
    public interface IQueryDispatcher<TEntity>
    {
        void RegisterHandler<TQuery>(Func<TQuery, Task<IList<TEntity>>> handler)
            where TQuery : BaseQuery;
        Task<IList<TEntity>> SendAsync(BaseQuery query);
    }
}
