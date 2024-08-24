using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewPostController : ControllerBase
    {
        private readonly ILogger<NewPostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public NewPostController(
            ILogger<NewPostController> logger,
            ICommandDispatcher commandDispatcher
        )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> Post(NewPostCommand command)
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
                _logger.LogError(ex, "Error creating post");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new NewPostResponse { Id = id, Message = "Error creating post", }
                );
            }
        }
    }
}
