using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Update.Internal;
using RAWANi.WEBAPi.Application.Contracts.UserProfileDtos.Requests;
using RAWANi.WEBAPi.Application.MEDiatR.UserProfileMDIR.Queries;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Extensions;
using RAWANi.WEBAPi.Filters;

namespace RAWANi.WEBAPi.Controllers.V1.UserProfiles
{
    [ApiVersion("1")]
    [Route(ApiRoutes.BaseRoute)]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class UserProfilesController : BaseController<UserProfilesController>
    {
        private readonly IAppLogger<UserProfilesController> _logger;
        private readonly IMediator _mediator;
        public UserProfilesController(IAppLogger<UserProfilesController> appLogger,
            IMediator mediator) : base(appLogger)
        {
            _logger = appLogger;
            _mediator = mediator;
        }

        [HttpGet(ApiRoutes.UserProfileRoutes.UserIdRoute, Name = "GetUserProfileByUserID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserProfileByUserID(
            [FromRoute] string userId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation ($"Geting user profile for user {userId}");
            var query = new GetUserProfileByUserIDQuery() { UserID = userId };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("The user profiles retrieved successfully.");
            return Ok(result);
        }

        [HttpGet(ApiRoutes.UserProfileRoutes.CurrentUserProfile, Name = "GetCurrentUserProfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfil(
            CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            if (userProfileId == null)
            {
                return BadRequest(new Error(
                    ErrorCode.BadRequest,
                    "Invalid Claim",
                    "The UserProfileID claim is missing or invalid."
                ));
            }

            _logger.LogInformation($"Geting user profile for  {userProfileId}");
            var query = new GetCurrentUserProfileQuery() { UserProfileID = userProfileId.Value };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("The user profile retrieved successfully.");
            return Ok(result);
        }

        [HttpPut(ApiRoutes.UserProfileRoutes.UserProfileIdRoute, Name = "UpdateUserProfileByProfileID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateGuid("userProfileId")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUserProfileByProfileID(
            [FromRoute] string userProfileId,
            [FromBody] UserProfileUpdateDto profileUpdateDto)
        {
            throw new NotImplementedException();
        }


    }
}
