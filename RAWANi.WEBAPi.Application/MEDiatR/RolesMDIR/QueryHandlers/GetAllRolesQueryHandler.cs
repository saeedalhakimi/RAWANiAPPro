using MediatR;
using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Queries;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.QueryHandlers
{
    public class GetAllRolesQueryHandler
        : IRequestHandler<GetAllRolesQuery, OperationResult<IEnumerable<IdentityRole>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<GetAllRolesQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetAllRolesQueryHandler(
            RoleManager<IdentityRole> roleManager,
            IAppLogger<GetAllRolesQueryHandler> logger,
            ILoggMessagingService messagingService,
            IErrorHandler errorHandler)
        {
            _roleManager = roleManager;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }
        public async Task<OperationResult<IEnumerable<IdentityRole>>> Handle(
            GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            // Log the start of the request handling
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(GetAllRolesQuery) }));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Retrieve all roles
                _logger.LogInformation("Retrieving all roles...");
                var roles = _roleManager.Roles.AsEnumerable(); // Use IEnumerable instead of List

                _logger.LogInformation($"Successfully retrieved roles.");

                // Step 2: Return the IEnumerable of roles
                return OperationResult<IEnumerable<IdentityRole>>.Success(roles);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation was canceled.");
                return _errorHandler.HandleCancelationToken<IEnumerable<IdentityRole>>(ex);
            }
            catch (Exception ex)
            {
                // Log the exception and handle it
                _logger.LogError("An unexpected error occurred while retrieving all roles.", ex);
                return _errorHandler.HandleException<IEnumerable<IdentityRole>>(ex);
            }
        }
    }
}
