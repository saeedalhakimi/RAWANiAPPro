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
    public class ForgotPasswordCommandHandler
        : IRequestHandler<ForgotPasswordCommand, OperationResult<bool>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IAppLogger<ForgotPasswordCommandHandler> _logger;
        private readonly ErrorHandler _errorHandler;

        public ForgotPasswordCommandHandler(
            UserManager<IdentityUser> userManager,
            IEmailService emailService,
            IAppLogger<ForgotPasswordCommandHandler> logger,
            ErrorHandler errorHandler)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to send password reset link to {request.Email}...");
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

                // Step 2: Generate a password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.LogInformation($"Password reset token generated for user {user.Email}.");

                // Step 3: Send the password reset link via email
                var resetLink = $"https://localhost:7015/api/v1/auth/reset-password?token={token}&email={request.Email}";
                var emailBody = $"Click the link to reset your password: {resetLink}";

               // await _emailService.SendEmailAsync(request.Email, "Password Reset", emailBody); use in production later
                _logger.LogInformation($"Password reset link sent to {request.Email}.");

                // Step 4: Simulate sending the email (for development)
                _logger.LogInformation($"Password reset link: {resetLink}");
                _logger.LogInformation($"Simulating email send to {request.Email} with reset link: {resetLink}");

                return OperationResult<bool>.Success(true);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to send a password reset link to {request.Email} was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while sending a password reset link to {request.Email}.", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
