using MediatR;
using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands;
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
    public class CreateRoleCommandHandler
        : IRequestHandler<CreateRoleCommand, OperationResult<bool>>
    {
        private readonly DataContext _ctx;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<CreateRoleCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;

        public CreateRoleCommandHandler(
            DataContext ctx, 
            RoleManager<IdentityRole> roleManager, 
            IAppLogger<CreateRoleCommandHandler> logger, 
            ILoggMessagingService MessagingService,
            ErrorHandler errorHandler)
        {
            _ctx = ctx;
            _roleManager = roleManager;
            _logger = logger;
            _messagingService = MessagingService;
            _errorHandler = errorHandler;
        }
        public async Task<OperationResult<bool>> Handle(
            CreateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(CreateRoleCommand) }));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Check if the role already exists
                _logger.LogInformation($"Checking if role '{request.RoleName}' already exists...");
                var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
                if (roleExists)
                {
                    _logger.LogWarning($"Role '{request.RoleName}' already exists.");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.BadRequest,
                        "Role Already Exists",
                        $"Role with name '{request.RoleName}' already exists"
                    ));
                }

                _logger.LogInformation($"Role '{request.RoleName}' does not exist. Proceeding with creation...");

                // Step 2: Create the role
                _logger.LogInformation($"Creating role '{request.RoleName}'...");
                var newRole = new IdentityRole(request.RoleName);
                var result = await _roleManager.CreateAsync(newRole);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to create role '{request.RoleName}'. Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        "Role Creation Failed",
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    ));
                }

                _logger.LogInformation($"Role '{request.RoleName}' created successfully.");
                return OperationResult<bool>.Success(result.Succeeded);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to create role '{request.RoleName}' was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                // Log the exception and handle it
                _logger.LogError($"An unexpected error occurred while creating role '{request.RoleName}'.", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
