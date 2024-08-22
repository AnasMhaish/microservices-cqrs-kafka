using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task On(PostCreatedEvent @event)
        {
            var post = new PostEntity
            {
                PostId = @event.Id,
                Author = @event.Author,
                Message = @event.Message,
                DatePosted = @event.DatePosted
            };
            await _postRepository.CreateAsync(post);
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);

            if (post == null)
            {
                throw new Exception("Post not found");
            }

            post.Message = @event.Message;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);

            if (post == null)
            {
                throw new Exception("Post not found");
            }

            post.Likes++;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity
            {
                CommentId = @event.CommentId,
                PostId = @event.Id,
                Username = @event.Username,
                Comment = @event.Comment,
                CommentDate = @event.CommentDate,
                IsEdited = false
            };

            await _commentRepository.CreateAsync(comment);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await _commentRepository.GetByIdAsync(@event.CommentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            comment.Comment = @event.Comment;
            comment.IsEdited = true;
            comment.CommentDate = @event.EditDate;

            await _commentRepository.UpdateAsync(comment);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await _commentRepository.DeleteAsync(@event.CommentId);
        }

        public async Task On(PostRemovedEvent @event)
        {
            await _postRepository.DeleteAsync(@event.Id);
        }
    }
}
