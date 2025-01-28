using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RAWANi.WEBAPi.Application.Contracts.RolesDto.Requests;
using RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands;
using RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Queries;
using RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.Queries;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Controllers.V1.Roles;
using RAWANi.WEBAPi.Filters;

namespace RAWANi.WEBAPi.Controllers.V1.Admin
{
    [ApiVersion("1")]
    [Route(ApiRoutes.BaseRoute)]
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : BaseController<AdminController>
    {
        private readonly IAppLogger<AdminController> _logger;
        private readonly IMediator _mediator;
        public AdminController(
            IAppLogger<AdminController> appLogger, 
            IMediator mediator) : base(appLogger)
        {
            _logger = appLogger;
            _mediator = mediator;
        }

        [HttpGet(ApiRoutes.AdminRoutes.RolesRoute, Name = "Admin_GetRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Admin_GetRoles()
        {
            var query = new GetAllRolesQuery();
            var result = await _mediator.Send(query);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AdminRoutes.RolesRoute, Name = "Admin_CreateRole")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Admin_CreateRole(
            [FromBody] CreateRoleDto createRoleDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new role.");
            var command = new CreateRoleCommand { RoleName = createRoleDto.RoleName };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role created successfully.");
            return CreatedAtRoute("Admin_CreateRole", result);
        }

        [HttpPut(ApiRoutes.AdminRoutes.RolesRoute, Name = "Admin_UpdateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Admin_UpdateRole(
            [FromRoute] string roleID,
            [FromBody] UpdateRoleDto updateRoleDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating role: {roleID}");
            var command = new UpdateRoleCommand
            {
                RoleID = roleID,
                NewRoleName = updateRoleDto.NewRoleName,
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role updated successfully.");
            return Ok(result);
        }

        [HttpDelete(ApiRoutes.AdminRoutes.RolesRoute, Name = "Admin_DeleteRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Admin_DeleteRole(
            [FromRoute] string roleID,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting Role: {roleID}");
            var command = new DeleteRoleCommand { RoleID = roleID };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return NoContent();
        }

        [HttpPost(ApiRoutes.AdminRoutes.AssignRole, Name = "Admin_AssignRoleToUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Admin_AssignRoleToUser(
            [FromBody] AssignOrRemoveRoleDto assignDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Assigning role..");
            var command = new AssignRoleToUserCommand
            {
                UserID = assignDto.UserID,
                RoleName = assignDto.RoleName
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role assigned successfully.");
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AdminRoutes.RemoveRole, Name = "Admin_RemoveRoleFromUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Admin_RemoveRoleFromUser(
            [FromBody] AssignOrRemoveRoleDto assignDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unassigning role..");
            var command = new RemoveRoleFromUserCommand
            {
                UserID = assignDto.UserID,
                RoleName = assignDto.RoleName
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role unassigned successfully.");
            return Ok(result);
        }

        [HttpGet(ApiRoutes.AdminRoutes.UserRouts, Name = "Admin_GetAllUsers")]
        public async Task<IActionResult> Admin_GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Geting details for all users...");
            var query = new GetAllUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Users details retrieved successfully.");
            return Ok(result);
        } 

        [HttpGet(ApiRoutes.AdminRoutes.UserIdRoute, Name = "Admin_GetUserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Admin_GetUserDetails(
            [FromRoute] string userId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Geting details for user: {userId}...");
            var query = new GetUserDetailsQuery() { UserID = userId };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("The user details retrieved successfully.");
            return Ok(result);
        }

        [HttpGet(ApiRoutes.AdminRoutes.UsernameRoute, Name = "Admin_GetUserByUsername")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Admin_GetUserByUsername(
            [FromBody] GetUserByUsernameQuery query,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Geting details for username: {query.UserName}...");
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("The user details retrieved successfully.");
            return Ok(result);
        }

    }
}
