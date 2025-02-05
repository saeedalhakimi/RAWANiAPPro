using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Abstractions;
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
    public class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, OperationResult<ResponseWithTokensDto>>
    {
        private readonly DataContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<LoginCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IJwtService _jwtService;
        private readonly IErrorHandler _errorHandler;
        public RefreshTokenCommandHandler(
            DataContext ctx,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAppLogger<LoginCommandHandler> appLogger,
            ILoggMessagingService messagingService,
            IJwtService jwtService,
            IErrorHandler errorHandler)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = appLogger;
            _messagingService = messagingService;
            _jwtService = jwtService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<ResponseWithTokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.Equals(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(RefreshTokenCommand) }));
            
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var token = request.RefreshToken;

                // Step 1: Validate the refresh token
                var refreshToken = await _ctx.RefreshTokens
                     .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsUsed && !rt.IsRevoked);
                if (refreshToken == null || refreshToken.IsUsed || refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.Unauthorized,
                        "Invalid Refresh Token",
                        "The refresh token is invalid or expired."
                    ));
                }

                // Step 2: Revoke the current refresh token
                _logger.LogInformation("Revoking refresh token for user: {UserId}", refreshToken.IdentityId);
                refreshToken.IsUsed = true;
                _ctx.RefreshTokens.Update(refreshToken);
                await _ctx.SaveChangesAsync(cancellationToken);

                // Step 3: Retrieve the user, roles and UserPrfile
                _logger.LogInformation("Retrieving user, roles and profile for user: {UserId}", refreshToken.IdentityId);
                var user = await _userManager.FindByIdAsync(refreshToken.IdentityId);
                if (user == null)
                {
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User not found",
                        "The user associated with the refresh token was not found."
                    ));
                }

                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                var profile = await _ctx.UserProfiles.FirstOrDefaultAsync(up => up.IdentityID == user.Id);
                if (profile == null) {
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User Profile not found",
                        "The user profile associated with the user was not found."
                    ));
                }

                // Step 4: Generate new tokens
                _logger.LogInformation("Generating access token for user: {UserId}", user.Id);
                var accessToken = _jwtService.GenerateAccessToken(user, profile, roles);

                // Step 5: Generate and store the new refresh token
                _logger.LogInformation("Generating refresh token for user: {UserId}", user.Id);
                var newRefreshToken = _jwtService.GenerateRefreshToken();
                var newRefreshTokenEntity = new RefreshToken
                {
                    Token = newRefreshToken,
                    ExpiryDate = _jwtService.GetRefreshTokenExpiryDate(),
                    IdentityId = user.Id
                };

                _logger.LogInformation("Storing refresh token for user: {UserId}", user.Id);
                await _ctx.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
                await _ctx.SaveChangesAsync(cancellationToken);

                return OperationResult<ResponseWithTokensDto>.Success(new ResponseWithTokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to refresh token was canceled.");
                return _errorHandler.HandleCancelationToken<ResponseWithTokensDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while refreshing the token.", ex);
                return _errorHandler.HandleException<ResponseWithTokensDto>(ex);
            }
        }
    }
}
