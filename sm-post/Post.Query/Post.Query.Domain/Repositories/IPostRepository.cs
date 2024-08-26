using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories
{
    public interface IPostRepository
    {
        Task CreateAsync(PostEntity post);
        Task UpdateAsync(PostEntity post);
        Task DeleteAsync(Guid postId);
        Task<PostEntity> GetByIdAsync(Guid postId);
        Task<IList<PostEntity>> GetAllAsync();
        Task<IList<PostEntity>> GetByAuthorAsync(string author);
        Task<IList<PostEntity>> GetWithLikesAsync(int numberOfLikes);
        Task<IList<PostEntity>> GetWithCommentsAsync();
    }
}
