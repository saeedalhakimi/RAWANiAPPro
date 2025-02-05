using MediatR;
using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.CommandHandlers
{
    public class DeleteRoleCommandHandler
        : IRequestHandler<DeleteRoleCommand, OperationResult<bool>>
    {
        private readonly DataContext _ctx;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<DeleteRoleCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;
        public DeleteRoleCommandHandler(
            DataContext ctx,
            RoleManager<IdentityRole> roleManager,
            IAppLogger<DeleteRoleCommandHandler> logger,
            ILoggMessagingService MessagingService,
            IErrorHandler errorHandler)
        {
            _ctx = ctx;
            _roleManager = roleManager;
            _logger = logger;
            _messagingService = MessagingService;
            _errorHandler = errorHandler;
        }
        public async Task<OperationResult<bool>> Handle(
            DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(DeleteRoleCommand) }));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Get role from Db
                _logger.LogInformation($"Attempting to retrieve role with ID: {request.RoleID}...");
                var role = await _roleManager.FindByIdAsync(request.RoleID);
                if (role == null)
                {
                    _logger.LogWarning($"Role with ID {request.RoleID} not found.");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.NotFound,
                        "Role Not Found",
                        "The specified role does not exist."
                    ));
                }

                _logger.LogInformation($"Role '{role.Name}' (ID: {role.Id}) retrieved successfully.");

                //Step 2: Deleting Role 
                _logger.LogInformation($"Attempting to delete role '{role.Name}' (ID: {role.Id})...");
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to delete role '{role.Name}' (ID: {role.Id}). Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        "Role Update Failed",
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    ));
                }

                _logger.LogInformation($"Role '{role.Name}' (ID: {role.Id}) deleted successfully.");
                return OperationResult<bool>.Success(result.Succeeded);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to delete role '{request.RoleID}' was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during role update: {request.RoleID}", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
