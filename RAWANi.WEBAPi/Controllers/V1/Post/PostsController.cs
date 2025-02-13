using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Requests;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Queries;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Extensions;
using RAWANi.WEBAPi.Filters;

namespace RAWANi.WEBAPi.Controllers.V1.Post
{
    [ApiVersion("1")]
    [Route(ApiRoutes.BaseRoute)]
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Authorize()]
    public class PostsController : BaseController<PostsController>
    {
        private readonly IMediator _mediator;
        private readonly IAppLogger<PostsController> _logger;
        public PostsController(
            IAppLogger<PostsController> appLogger, IMediator mediator)
            : base(appLogger)
        {
            _logger = appLogger;
            _mediator = mediator;
        }

        [HttpGet(Name = " GetPostsWithPagination")]
        public async Task<IActionResult> GetPostsWithPagination(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortColumn = "CreatedAt",
            [FromQuery] string sortDirection = "ASC",
            CancellationToken cancellationToken = default)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }

            var query = new GetPostsWithPaginationQuery
            {
                UserProfile = userProfileId.Value,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to retrieve posts. Errors: {Errors}",
                    string.Join(", ", result.Errors));
                return HandleErrorResponse(result);
            }

            return Ok(result);
        }

        [HttpGet(ApiRoutes.Posts.PostIdRoute, Name = "GetPostByID")]
        [ValidateGuid("postId")]
        public async Task<IActionResult> GetPostByID(
            [FromRoute] string postId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to process the retrieve request for post id: {postId}.", postId);

            _logger.LogInformation("Creating the GetPostByIDQuery.");
            var query = new GetPostByIDQuery
            {
                PostID = Guid.Parse(postId)
            };
            _logger.LogInformation("Sending the query to the mediator.");
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to retrieve the post. Errors: {Errors}",
                    string.Join(", ", result.Errors));
                return HandleErrorResponse(result);
            }

            _logger.LogInformation("Post retrieved successfully. PostID: {PostID}",
                postId);


            return Ok(result);

        }

        [HttpPost(Name = "CreatePost")]
        [ValidateModel]
        public async Task<IActionResult> CreatePost(
            [FromForm] CreatePostDto createPostDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to process the CreatePost request.");

            // Step 1: Retrieve the UserProfileID from the claims
            _logger.LogInformation("Retrieving UserProfileID from the claims.");

            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }
            _logger.LogInformation("UserProfileID retrieved successfully: {UserProfileID}", userProfileId);

            // Step 2: Create the CreatePostCommand
            _logger.LogInformation("Creating the CreatePostCommand.");
            var command = new CreatePostCommand
            {
                UserProfileID = userProfileId.Value,
                PostTitle = createPostDto.PostTitle,
                PostContent = createPostDto.PostContent,
                PostImage = createPostDto.PostImage,
            };
            _logger.LogInformation("CreatePostCommand created successfully.");

            // Step 3: Send the command to the mediator
            _logger.LogInformation("Sending the CreatePostCommand to the mediator.");
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create the post. Errors: {Errors}",
                    string.Join(", ", result.Errors));
                return HandleErrorResponse(result);
            }

            _logger.LogInformation("Post created successfully. PostID: {PostID}",
                result.Data?.PostID);

            // Step 4: Return the response
            _logger.LogInformation("Returning the created post response.");
            return CreatedAtRoute("CreatePost", result);
        }

        [HttpPut(ApiRoutes.Posts.PostIdRoute, Name = "UpdatePost")]
        [ValidateModel]
        [ValidateGuid("postId")]
        public async Task<IActionResult> UpdatePost(
            [FromRoute] string postId,
            [FromForm] UpdatePostContentsDto updatePostDto,
            CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }

            var command = new UpdatePostContentsCommand
            {
                PostID = Guid.Parse(postId),
                UserProfileID = userProfileId.Value,
                PostTitle = updatePostDto.PostTitle,
                PostContent = updatePostDto.PostContent,
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpPut(ApiRoutes.Posts.UpdatePostImage, Name = "UpdatePostImage")]
        [ValidateGuid("postId")]

        public async Task<IActionResult> UpdatePostImage(
            [FromRoute] string postId,
            [FromForm] UpdatePostImageDto updatePostImageDto,
            CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }

            var command = new UpdatePostImageCommand
            {
                PostID = Guid.Parse(postId),
                UserProfileID = userProfileId.Value,
                PostImage = updatePostImageDto.PostImage,
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpDelete(ApiRoutes.Posts.PostIdRoute, Name = "DeletePost")]
        [ValidateGuid("postId")]
        public async Task<IActionResult> DeletePost(
            [FromRoute] string postId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to process the DeletePost request.");

            // Step 1: Retrieve the UserProfileID from the claims
            _logger.LogInformation("Retrieving UserProfileID from the claims.");

            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }

            // Step 2: Create the CreatePostCommand
            _logger.LogInformation("Creating the DeletePostCommand.");
            var command = new DeletePostCommand
            {
                UserProfileID = userProfileId.Value,
                PostID = Guid.Parse(postId)
            };
            _logger.LogInformation("DeletePostCommand created successfully.");

            // Step 3: Send the command to the mediator
            _logger.LogInformation("Sending the DeletePostCommand to the mediator.");
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to Delete the post. Errors: {Errors}",
                    string.Join(", ", result.Errors));
                return HandleErrorResponse(result);
            }

            _logger.LogInformation("Post deleted successfully. PostID: {PostID}",
                postId);

            // Step 4: Return the response
            return Ok(result);

        }

        [HttpPost(ApiRoutes.Posts.PostCommentsRoute, Name = "CreatePostComment")]
        [ValidateGuid("postId")]
        public async Task<IActionResult> CreatePostComment(
            [FromRoute] string postId,
            [FromBody] CreatePostCommentDto createPostCommentDto,
            CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }

            var command = new CreatePostCommentCommand
            {
                PostID = Guid.Parse(postId),
                UserProfileID = userProfileId.Value,
                CommentContent = createPostCommentDto.CommentContent,
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return CreatedAtRoute("CreatePostComment", result);
        }

        [HttpGet(ApiRoutes.Posts.PostCommentsRoute, Name = "GetPostComments")]
        [ValidateGuid("postId")]
        public async Task<IActionResult> GetPostComments(
            [FromRoute] string postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortColumn = "CreatedAt",
            [FromQuery] string sortDirection = "ASC",
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllCommentsForAPostQuery
            {
                PostID = Guid.Parse(postId),
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.Posts.PostOneCommentIdRoute, Name = "GetAPostComment")]
        [ValidateGuid("postId")]
        [ValidateGuid("commentId")]
        public async Task<IActionResult> GetAPostComment(
            [FromRoute] string postId,
            [FromRoute] string commentId,
            CancellationToken cancellationToken)
        {
            var query = new GetAPostCommentQuery
            {
                PostId = Guid.Parse(postId),
                CommentId = Guid.Parse(commentId)
            };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.Posts.PostWithCommentsRoute, Name = "GetPostWithComments")]
        [ValidateGuid("postId")]
        public async Task<IActionResult> GetPostWithComments(
            [FromRoute] string postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortColumn = "CreatedAt",
            [FromQuery] string sortDirection = "ASC",
            CancellationToken cancellationToken = default)
        {
            var query = new GetPostWithCommentsQuery
            {
                PostID = Guid.Parse(postId),
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.Posts.CommentIDRoute, Name = "GetCommentByID")]
        [ValidateGuid("commentId")]
        public async Task<IActionResult> GetCommentByID(
            [FromRoute] string commentId,
            CancellationToken cancellationToken)
        {
            var query = new GetCommentByIDQuery
            {
                CommentID = Guid.Parse(commentId)
            };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpPut(ApiRoutes.Posts.CommentIDRoute, Name = "UpdateComment")]
        [ValidateGuid("commentId")]
        public async Task<IActionResult> UpdateComment(
            [FromRoute] string commentId,
            [FromBody] UpdateCommentDto updateCommentDto,
            CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }
            var command = new UpdatePostCommentCommand
            {
                CommentID = Guid.Parse(commentId),
                UserProfileID = userProfileId.Value,
                CommentContent = updateCommentDto.CommentContent
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpDelete(ApiRoutes.Posts.CommentIDRoute, Name = "DeleteComment")]
        [ValidateGuid("commentId")]
        public async Task<IActionResult> DeleteComment(
            [FromRoute] string commentId,
            CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                _logger.LogWarning("Invalid or missing UserProfileID claim.");
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }
            var command = new DeletePostCommentCommand
            {
                CommentID = Guid.Parse(commentId),
                UserProfileID = userProfileId.Value
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }
    }
}
