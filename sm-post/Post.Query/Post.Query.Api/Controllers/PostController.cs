using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostController(
            ILogger<PostController> logger,
            IQueryDispatcher<PostEntity> queryDispatcher
        )
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        private IActionResult ErrorResponse(Exception ex, string message)
        {
            _logger.LogError(ex, message);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new BaseResponse { Message = message }
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());

                if (posts == null || !posts.Any())
                    return NoContent();

                var count = posts.Count;
                return Ok(
                    new PostResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved {count} posts."
                    }
                );
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyPostIdAsync(Guid id)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery { Id = id });

                if (posts == null || !posts.Any())
                    return NoContent();

                var count = posts.Count;
                return Ok(
                    new PostResponse { Posts = posts, Message = $"Successfully returned post." }
                );
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving a post.");
            }
        }

        [HttpGet("author/{author}")]
        public async Task<IActionResult> GetByAuthorAsync(string author)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(
                    new FindPostByAuthorQuery { Author = author }
                );

                if (posts == null || !posts.Any())
                    return NoContent();

                var count = posts.Count;
                return Ok(
                    new PostResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved {count} posts by author {author}."
                    }
                );
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts by author.");
            }
        }

        [HttpGet("likes/{numberOfLikes}")]
        public async Task<IActionResult> GetWithLikesAsync(int numberOfLikes)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(
                    new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes }
                );

                if (posts == null || !posts.Any())
                    return NoContent();

                var count = posts.Count;
                return Ok(
                    new PostResponse
                    {
                        Posts = posts,
                        Message =
                            $"Successfully retrieved {count} posts with {numberOfLikes} likes."
                    }
                );
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts with likes.");
            }
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetWithCommentsAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

                if (posts == null || !posts.Any())
                    return NoContent();

                var count = posts.Count;
                return Ok(
                    new PostResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved {count} posts with comments."
                    }
                );
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts with comments.");
            }
        }
    }
}
