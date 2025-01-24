using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.CommandHandlers
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, OperationResult<ResponseWithTokensDto>>
    {
        private readonly DataContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<LoginCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly JwtService _jwtService;
        private readonly ErrorHandler _errorHandler;

        public LoginCommandHandler(
            DataContext ctx,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAppLogger<LoginCommandHandler> appLogger,
            ILoggMessagingService messagingService,
            JwtService jwtService,
            ErrorHandler errorHandler)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = appLogger;
            _messagingService = messagingService;
            _jwtService = jwtService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<ResponseWithTokensDto>> Handle(
            LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(LoginCommand) }));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Find the user account
                _logger.LogInformation("Attempting to find user by username: {Username}", request.Username);
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                {
                    _logger.LogWarning("User not found for username: {Username}", request.Username);
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.NotFound, "User Not Found", "The provided email address does not match any account."));
                }

                // Step 2: Check if the user account is disabled
                _logger.LogInformation("Checking if user email is confirmed for user: {UserId}", user.Id);
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogWarning("User account is disabled for user: {UserId}", user.Id);
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.Unauthorized, "Account Disabled", "The account is disabled. Please confirm your email address."));
                }

                // Step 3: Check if the user account is locked
                _logger.LogInformation("Checking if user account is locked for user: {UserId}", user.Id);
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("User account is locked for user: {UserId}", user.Id);
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.LockedOut, "Account Locked", "The account is locked. Please contact the administrator."));
                }

                
                // Step 4: Check if the password is correct
                _logger.LogInformation("Validating password for user: {UserId}", user.Id);
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    _logger.LogWarning("Invalid password provided for user: {UserId}", user.Id);
                    await _userManager.AccessFailedAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        _logger.LogWarning("User account is now locked due to multiple failed login attempts: {UserId}", user.Id);
                        return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                            ErrorCode.LockedOut, "Account Locked", "The account is locked. Please contact the administrator."));
                    }

                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.Unauthorized, "Invalid Credentials", "The provided password is incorrect."));
                }

                // Step 5: Retrieve the user's roles
                _logger.LogInformation("Retrieving roles for user: {UserId}", user.Id);
                var roles = (await _userManager.GetRolesAsync(user)).ToList();

                // Step 6: Retrieve the user profile
                _logger.LogInformation("Retrieving user profile for user: {UserId}", user.Id);
                var profile = await _ctx.UserProfiles.FirstOrDefaultAsync(up => up.IdentityID == user.Id, cancellationToken);
                if (profile == null)
                {
                    _logger.LogWarning("User profile not found for user: {UserId}", user.Id);
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.NotFound, "User Profile Not Found", "The user profile for the provided account could not be found."));
                }

                // Step 7: Generate the JWT tokens
                _logger.LogInformation("Generating access token for user: {UserId}", user.Id);
                var accessToken = _jwtService.GenerateAccessToken(user, profile, roles);

                // Step 8: Generate and store the refresh token
                _logger.LogInformation("Generating refresh token for user: {UserId}", user.Id);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    ExpiryDate = _jwtService.GetRefreshTokenExpiryDate(), // Use the new method
                    IdentityId = user.Id
                };

                _logger.LogInformation("Storing refresh token for user: {UserId}", user.Id);
                await _ctx.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
                await _ctx.SaveChangesAsync(cancellationToken);

                // Step 9: Return the response
                _logger.LogInformation("Login successful for user: {UserId}", user.Id);
                return OperationResult<ResponseWithTokensDto>.Success(new ResponseWithTokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to login '{request.Username}' was canceled.");
                return _errorHandler.HandleCancelationToken<ResponseWithTokensDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during login for username: {request.Username}", ex );
                return _errorHandler.HandleException<ResponseWithTokensDto>(ex);
            }
        }
    }
}