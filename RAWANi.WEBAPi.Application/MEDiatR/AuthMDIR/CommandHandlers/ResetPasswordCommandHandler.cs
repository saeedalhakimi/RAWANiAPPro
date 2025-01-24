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
    public class ResetPasswordCommandHandler
        : IRequestHandler<ResetPasswordCommand, OperationResult<bool>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppLogger<ResetPasswordCommandHandler> _logger;
        private readonly ErrorHandler _errorHandler;

        public ResetPasswordCommandHandler(
            UserManager<IdentityUser> userManager,
            IAppLogger<ResetPasswordCommandHandler> logger,
            ErrorHandler errorHandler)
        {
            _userManager = userManager;
            _logger = logger;
            _errorHandler = errorHandler;
        }
        public async Task<OperationResult<bool>> Handle(
            ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to reset password for {request.Email}...");
            try
            {
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

                // Step 2: Reset the password using the token
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to reset password for {request.Email}. Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        "Password Reset Failed",
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    ));
                }

                _logger.LogInformation($"Password reset successfully for {request.Email}.");

                return OperationResult<bool>.Success(result.Succeeded);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to reset password for {request.Email} was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while resetting the password for {request.Email}.", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
