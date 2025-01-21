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
            _logger.LogInformation(_messagingService.GetLoggMessage( nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                //Step 1: find the user account
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null) return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                       ErrorCode.NotFound, "User Not Found", "The provided email address does not match any account."));

                //Step 2: Check if the user account is locked
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.LockedOut, "Account Locked", "The account is locked. Please contact the administrator."));
                }

                //Step 3: Check if the user account is disabled
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.Unauthorized, "Account Disabled", "The account is disabled. Please contact the administrator."));
                }

                //Step 4: Check if the password is correct
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    await _userManager.AccessFailedAsync(user);
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                            ErrorCode.LockedOut, "Account Locked", "The account is locked. Please contact the administrator."));
                    }
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.Unauthorized, "Invalid Credentials", "The provided password is incorrect."));
                }

                //Step 5: Retrieve the user's roles
                var roles = (await _userManager.GetRolesAsync(user)).ToList();

                //Step 6: Retrieve the user profile
                var profile = await _ctx.UserProfiles.FirstOrDefaultAsync(up => up.IdentityID == user.Id, cancellationToken);
                if (profile == null) return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.NotFound, "User Profile Not Found", "The user profile for the provided account could not be found."));


                //Step 7: Generate the JWT tokens
                var accessToken = _jwtService.GenerateAccessToken(user, profile, roles);

                //Step 8: Generate and store the refresh token
                var refreshToken = _jwtService.GenerateRefreshToken();
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    ExpiryDate = _jwtService.GetRefreshTokenExpiryDate(), // Use the new method
                    IdentityId = user.Id
                };

                await _ctx.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
                await _ctx.SaveChangesAsync(cancellationToken);

                //Step 9: Return the response
                return OperationResult<ResponseWithTokensDto>.Success(new ResponseWithTokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });

            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<ResponseWithTokensDto>(ex);
            }
        }
    }
}
