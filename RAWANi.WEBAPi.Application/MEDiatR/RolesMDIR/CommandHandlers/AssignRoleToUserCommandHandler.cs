using MediatR;
using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.CommandHandlers;
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
    public class AssignRoleToUserCommandHandler
        : IRequestHandler<AssignRoleToUserCommand, OperationResult<bool>>
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<AssignRoleToUserCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;
        public AssignRoleToUserCommandHandler(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAppLogger<AssignRoleToUserCommandHandler> appLogger,
            ILoggMessagingService messagingService,
            ErrorHandler errorHandler)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }
        public async Task<OperationResult<bool>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Find the user by ID
                _logger.LogInformation($"Attempting to find user by ID: {request.UserID}...");
                var user = await _userManager.FindByIdAsync(request.UserID);
                if (user == null) 
                {
                    _logger.LogWarning($"User with ID {request.UserID} not found.");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User Not Found",
                        $"User with ID {request.UserID} does not exist."
                    ));
                }

                _logger.LogInformation($"User '{user.UserName}' (ID: {user.Id}) found successfully.");

                // Step 2: Check if the role exists
                _logger.LogInformation($"Checking if role '{request.RoleName}' exists...");
                var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
                if (!roleExists)
                {
                    _logger.LogWarning($"Role '{request.RoleName}' does not exist.");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.NotFound,
                        "Role Not Found",
                        $"Role '{request.RoleName}' does not exist."
                    ));
                }

                _logger.LogInformation($"Role '{request.RoleName}' exists.");

                // Step 3: Assign the role to the user
                _logger.LogInformation($"Attempting to assign role '{request.RoleName}' to user '{user.UserName}' (ID: {user.Id})...");
                var result = await _userManager.AddToRoleAsync(user, request.RoleName);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to assign role '{request.RoleName}' to user '{user.UserName}' (ID: {user.Id}). Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        "Role Assignment Failed",
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    ));
                }

                _logger.LogInformation($"Role '{request.RoleName}' assigned to user '{user.UserName}' (ID: {user.Id}) successfully.");

                // Step 4: Return success
                return OperationResult<bool>.Success(result.Succeeded);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to assign role '{request.RoleName}' to user '{request.UserID}' was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while assigning role '{request.RoleName}' to user '{request.UserID}'.", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
