using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public PostController(ILogger<PostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync(NewPostCommand command)
        {
            Guid id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new NewPostResponse { Message = "Post created successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to create a post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new NewPostResponse
                    {
                        Id = id,
                        Message = "Error while processing request to create a post"
                    }
                );
            }
        }

        [HttpPut("{id}/message")]
        public async Task<IActionResult> UpdatePostMessageAsync(Guid id, EditMessageCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(
                    StatusCodes.Status200OK,
                    new BaseResponse { Message = "Message edited successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogError(
                    ex,
                    "Could not retrieve aggregate, client passed an invalid post Id targetting the aggregate"
                );
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to update post message");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new BaseResponse
                    {
                        Message = "Error while processing request to update post message"
                    }
                );
            }
        }

        [HttpPut("{id}/like")]
        public async Task<IActionResult> LikePostAsync(Guid id)
        {
            try
            {
                await _commandDispatcher.SendAsync(new LikePostCommand { Id = id, });

                return StatusCode(
                    StatusCodes.Status200OK,
                    new BaseResponse { Message = "Post Liked successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogError(
                    ex,
                    "Could not retrieve aggregate, client passed an invalid post Id targetting the aggregate"
                );
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to like a post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new BaseResponse { Message = "Error while processing request to like a post" }
                );
            }
        }

        [HttpPost("{id}/comment")]
        public async Task<IActionResult> CreateCommentAsync(Guid id, AddCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new NewPostResponse { Message = "Post Comment created successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to create a comment for post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new NewPostResponse
                    {
                        Id = id,
                        Message = "Error while processing request to create a comment for post"
                    }
                );
            }
        }

        [HttpPut("{id}/comment/{commentId}")]
        public async Task<IActionResult> EditCommentAsync(
            Guid id,
            Guid commentId,
            EditCommentCommand command
        )
        {
            try
            {
                command.Id = id;
                command.CommentId = commentId;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new NewPostResponse { Message = "Edit Comment successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to edit a comment for post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new NewPostResponse
                    {
                        Id = id,
                        Message = "Error while processing request to edit a comment for post"
                    }
                );
            }
        }

        [HttpDelete("{id}/comment/{commentId}")]
        public async Task<IActionResult> RemoveCommentAsync(
            Guid id,
            Guid commentId,
            RemoveCommentCommand command
        )
        {
            try
            {
                command.Id = id;
                command.CommentId = commentId;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new NewPostResponse { Message = "Remove Comment successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to remove a comment for post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new NewPostResponse
                    {
                        Id = id,
                        Message = "Error while processing request to remove a comment for post"
                    }
                );
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync(Guid id, DeletePostCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new NewPostResponse { Message = "Remove Post successfully", }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Client made a bad request");
                return BadRequest(new BaseResponse { Message = ex.Message, });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request to remove a post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new NewPostResponse
                    {
                        Id = id,
                        Message = "Error while processing request to remove a post"
                    }
                );
            }
        }
    }
}
