using MediatR;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses;
using RAWANi.WEBAPi.Application.Data.DbContexts;
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
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, OperationResult<bool>>
    {
        private readonly DataContext _ctx;
        private readonly IAppLogger<LoginCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;
        public LogoutCommandHandler(
            DataContext ctx,
            IAppLogger<LoginCommandHandler> appLogger,
            ILoggMessagingService messagingService,
            ErrorHandler errorHandler)
        {
            _ctx = ctx;
            _logger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<bool>> Handle(
            LogoutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(LogoutCommand) }));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var token = request.RefreshToken;

                // Step 1: Validate the refresh token
                var refreshToken = await _ctx.RefreshTokens
                     .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsUsed && !rt.IsRevoked);
                if (refreshToken == null || refreshToken.IsUsed || refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.Unauthorized,
                        "Invalid Refresh Token",
                        "The refresh token is invalid or expired."
                    ));
                }

                // Step 2: Revoke the refresh token
                _logger.LogInformation("Revoking refresh token for user: {UserId}", refreshToken.IdentityId);
                refreshToken.IsRevoked = true;
                _ctx.RefreshTokens.Update(refreshToken);
                await _ctx.SaveChangesAsync(cancellationToken);

                return OperationResult<bool>.Success(true);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to logout user was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
