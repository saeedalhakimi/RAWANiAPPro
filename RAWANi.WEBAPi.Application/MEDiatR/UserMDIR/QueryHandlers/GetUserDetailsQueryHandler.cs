using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.UsersDto;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.CommandHandlers;
using RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.Queries;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.QueryHandlers
{
    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, OperationResult<UserResponseDto>>
    {
        private readonly DataContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<GetUserDetailsQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetUserDetailsQueryHandler(
            DataContext ctx, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IAppLogger<GetUserDetailsQueryHandler> logger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<UserResponseDto>> Handle(
            GetUserDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Find the user by ID
                _logger.LogInformation($"Retrieving details for user ID: {request.UserID}...");
                var user = await _userManager.FindByIdAsync(request.UserID);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {request.UserID} not found.");
                    return OperationResult<UserResponseDto>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User Not Found",
                        $"User with ID {request.UserID} does not exist."
                    ));
                }

                // Step 2: Get user roles
                var roles = (await _userManager.GetRolesAsync(user)).ToList();

                // Step 3: Find all user profiles for the given user ID
                _logger.LogInformation($"Retrieving user profiles for user ID: {request.UserID}...");
                var userProfiles = await _ctx.UserProfiles
                    .Include(up => up.BasicInfo) // Include basic information
                    .Where(up => up.IdentityID == request.UserID) // Filter by user ID
                    .ToListAsync(cancellationToken);

                var response = UserMappers.ToUserResponseDto(user, roles, userProfiles);

                _logger.LogInformation($"User details for ID {request.UserID} retrieved successfully.");
                return OperationResult<UserResponseDto>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to retrieve user details for ID {request.UserID} was canceled.");
                return _errorHandler.HandleCancelationToken<UserResponseDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user details for ID {request.UserID}.", ex);
                return _errorHandler.HandleException<UserResponseDto>(ex);
            }
        }
    }
}
