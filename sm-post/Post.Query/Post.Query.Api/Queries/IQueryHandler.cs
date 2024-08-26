using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries
{
    public interface IQueryHandler
    {
        Task<IList<PostEntity>> HandleAsync(FindPostByIdQuery query);
        Task<IList<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query);
        Task<IList<PostEntity>> HandleAsync(FindPostsWithLikesQuery query);
        Task<IList<PostEntity>> HandleAsync(FindPostByAuthorQuery query);
        Task<IList<PostEntity>> HandleAsync(FindAllPostsQuery query);
    }
}
