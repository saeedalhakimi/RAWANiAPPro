using MediatR;
using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.CommandHandlers
{
    public class ConfirmEmailCommandHandler
    : IRequestHandler<ConfirmEmailCommand, OperationResult<bool>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppLogger<ConfirmEmailCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;

        public ConfirmEmailCommandHandler(
            UserManager<IdentityUser> userManager,
            IAppLogger<ConfirmEmailCommandHandler> logger,
            ILoggMessagingService messagingService,
            ErrorHandler errorHandler)
        {
            _userManager = userManager;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                _logger.LogInformation($"Attempting to confirm email for {request.Email}...");
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Find the user by email
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning($"User with email {request.Email} not found.");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User Not Found",
                        $"User with email {request.Email} does not exist."
                    ));
                }

                // Step 2: Confirm the email using the token
                var result = await _userManager.ConfirmEmailAsync(user, request.Token);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to confirm email for {request.Email}. Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        "Email Confirmation Failed",
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    ));
                }

                _logger.LogInformation($"Email confirmed successfully for {request.Email}.");

                return OperationResult<bool>.Success(true);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to confirm email for {request.Email} was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while confirming email for {request.Email}.",ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
