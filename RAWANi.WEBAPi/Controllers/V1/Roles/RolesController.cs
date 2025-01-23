using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RAWANi.WEBAPi.Application.Contracts.RolesDto.Requests;
using RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands;
using RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Queries;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Filters;

namespace RAWANi.WEBAPi.Controllers.V1.Roles
{
    [ApiVersion("1")]
    [Route(ApiRoutes.BaseRoute)]
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class RolesController : BaseController<RolesController>
    {
        private readonly IAppLogger<RolesController> _logger;
        private readonly IMediator _mediator;
        public RolesController(
            IAppLogger<RolesController> appLogger,
            IMediator mediator) : base(appLogger)
        {
            _logger = appLogger;
            _mediator = mediator;
        }

        [HttpGet(Name = "GetRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles()
        {
            var query = new GetAllRolesQuery();
            var result = await _mediator.Send(query);
            if(!result.IsSuccess) return HandleErrorResponse(result);
            return Ok(result);
        }

        [HttpPost(Name = "CreateRole")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> CreateRole(
            [FromBody] CreateRoleDto createRoleDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new role.");
            var command = new CreateRoleCommand()
            {
                RoleName = createRoleDto.RoleName,
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role created successfully.");
            return CreatedAtRoute("CreateRole", result);

        }

        [HttpPut(ApiRoutes.Roles.RoleIDRoute, Name = "RoleIDRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> UpdateRole(
            [FromRoute] string roleID,
            [FromBody] UpdateRoleDto updateRoleDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating role: {roleID}");
            var command = new UpdateRoleCommand()
            {
                RoleID = roleID,
                NewRoleName = updateRoleDto.NewRoleName,
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role updated successfully.");
            return Ok(result);
        }

        [HttpDelete(ApiRoutes.Roles.RoleIDRoute, Name = "DeleteRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRole(
            [FromRoute] string roleID,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting Role: {roleID}");
            var command = new DeleteRoleCommand() { RoleID = roleID, };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            return NoContent();
        }

        [HttpPost(ApiRoutes.Roles.AssignRole, Name = "AssignRoleToUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> AssignRoleToUser(
            [FromBody] AssignOrRemoveRoleDto assignDto, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("assigning role..");
            var command = new AssignRoleToUserCommand()
            {
                UserID = assignDto.UserID,
                RoleName = assignDto.RoleName
            };
            var result = await _mediator.Send(command,cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role assigned successfully.");
            return Ok(result);
        }

        [HttpPost(ApiRoutes.Roles.RemoveRole, Name = "RemoveRoleFromUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> RemoveRoleFromUser(
            [FromBody] AssignOrRemoveRoleDto assignDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unassigning role..");
            var command = new RemoveRoleFromUserCommand() { UserID = assignDto.UserID, RoleName = assignDto.RoleName };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Role unassigned successfully.");
            return Ok(result);
        }
    }
}
