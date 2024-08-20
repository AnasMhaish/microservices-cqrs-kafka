using System.Runtime.CompilerServices;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active
        {
            get => _active;
            set => _active = value;
        }

        public PostAggregate() { }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(
                new PostCreatedEvent
                {
                    Id = id,
                    Author = author,
                    Message = message,
                    DatePosted = DateTime.UtcNow
                }
            );
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _author = @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException(
                    "You cannot edit the message of an inactive post!"
                );
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}!"
                );
            }

            RaiseEvent(new MessageUpdatedEvent { Id = _id, Message = message });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot like an inactive post!");
            }

            RaiseEvent(new PostLikedEvent { Id = _id });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        public void AddComment(string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot comment on an inactive post!");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!"
                );
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!"
                );
            }

            RaiseEvent(
                new CommentAddedEvent
                {
                    Id = _id,
                    CommentId = Guid.NewGuid(),
                    Comment = comment,
                    Username = username,
                    CommentDate = DateTime.UtcNow
                }
            );
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(
                @event.CommentId,
                new Tuple<string, string>(@event.Comment, @event.Username)
            );
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException(
                    "You cannot edit a comment on an inactive post!"
                );
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!"
                );
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!"
                );
            }

            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException(
                    $"The comment with the id {commentId} does not exist. Please provide a valid comment id!"
                );
            }

            if (!_comments[commentId].Item2.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You cannot edit a comment that created by another user!"
                );
            }

            RaiseEvent(
                new CommentUpdatedEvent
                {
                    Id = _id,
                    CommentId = commentId,
                    Comment = comment,
                    Username = username,
                    EditDate = DateTime.UtcNow
                }
            );
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(
                @event.Comment,
                @event.Username
            );
        }

        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException(
                    "You cannot remove a comment on an inactive post!"
                );
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!"
                );
            }

            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException(
                    $"The comment with the id {commentId} does not exist. Please provide a valid comment id!"
                );
            }

            if (!_comments[commentId].Item2.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You cannot remove a comment that created by another user!"
                );
            }

            RaiseEvent(new CommentRemovedEvent { Id = _id, CommentId = commentId });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(string usename)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot delete an inactive post!");
            }

            if (string.IsNullOrWhiteSpace(usename))
            {
                throw new InvalidOperationException(
                    $"The value of {nameof(usename)} cannot be null or empty. Please provide a valid {nameof(usename)}!"
                );
            }

            if (!_author.Equals(usename, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You cannot delete a post that created by another user!"
                );
            }

            RaiseEvent(new PostRemovedEvent { Id = _id });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
