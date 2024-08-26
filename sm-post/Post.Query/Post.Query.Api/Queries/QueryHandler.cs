using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPostRepository _postRepository;

        public QueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IList<PostEntity>> HandleAsync(FindPostByIdQuery query)
        {
            var post = await _postRepository.GetByIdAsync(query.Id);
            return new List<PostEntity> { post };
        }

        public async Task<IList<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
        {
            return await _postRepository.GetWithCommentsAsync();
        }

        public async Task<IList<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
        {
            return await _postRepository.GetWithLikesAsync(query.NumberOfLikes);
        }

        public async Task<IList<PostEntity>> HandleAsync(FindPostByAuthorQuery query)
        {
            return await _postRepository.GetByAuthorAsync(query.Author);
        }

        public async Task<IList<PostEntity>> HandleAsync(FindAllPostsQuery query)
        {
            return await _postRepository.GetAllAsync();
        }
    }
}
