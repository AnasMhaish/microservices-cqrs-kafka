using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory _contextFactory;

        public PostRepository(DatabaseContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateAsync(PostEntity post)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Posts.Add(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            using var context = _contextFactory.CreateDbContext();
            var post = await GetByIdAsync(postId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task<IList<PostEntity>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context
                .Posts.AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<PostEntity>> GetByAuthorAsync(string author)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context
                .Posts.AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .Where(p => p.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context
                .Posts.Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<IList<PostEntity>> ListWithCommentsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context
                .Posts.AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .Where(x => x.Comments != null && x.Comments.Any())
                .ToListAsync();
        }

        public async Task<IList<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context
                .Posts.AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .Where(x => x.Likes >= numberOfLikes)
                .ToListAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Posts.Update(post);
            _ = await context.SaveChangesAsync();
        }
    }
}
