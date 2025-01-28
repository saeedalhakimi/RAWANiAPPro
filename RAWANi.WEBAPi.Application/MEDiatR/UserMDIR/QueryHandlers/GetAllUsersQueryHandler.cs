using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Application.Data.DbContexts;
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
    public class GetAllUsersQueryHandler 
        : IRequestHandler<GetAllUsersQuery, OperationResult<PagedResponse<UserResponseDto>>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DataContext _ctx;
        private readonly IAppLogger<GetAllUsersQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;

        public GetAllUsersQueryHandler(
            UserManager<IdentityUser> userManager,
            DataContext ctx,
            IAppLogger<GetAllUsersQueryHandler> logger,
            ILoggMessagingService messagingService,
            ErrorHandler errorHandler)
        {
            _userManager = userManager;
            _ctx = ctx;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }
        public async Task<OperationResult<PagedResponse<UserResponseDto>>> Handle(
            GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogInformation("Retrieving all users with pagination...");

                // Step 1: Get the total count of users
                var totalCount = await _userManager.Users.CountAsync(cancellationToken);

                // Step 2: Retrieve paginated users
                var users = await _userManager.Users
                    .Skip((request.PageNumber - 1) * request.PageSize) // Skip records for previous pages
                    .Take(request.PageSize) // Take records for the current page
                    .ToListAsync(cancellationToken);

                // Step 3: Retrieve user profiles for the paginated users
                var userProfiles = await _ctx.UserProfiles
                    .Include(up => up.BasicInfo) // Include basic information
                    .Where(up => users.Select(u => u.Id).Contains(up.IdentityID)) // Filter by user IDs
                    .ToListAsync(cancellationToken);

                // Step 4: Map users to DTOs with their profiles
                var userDtos = users.Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList(), // Get user roles
                    UserProfiles = userProfiles
                        .Where(up => up.IdentityID == user.Id) // Filter profiles for the current user
                        .ToList()
                }).ToList();

                // Step 5: Create the paginated response
                var pagedResponse = new PagedResponse<UserResponseDto>(
                    userDtos,
                    request.PageNumber,
                    request.PageSize,
                    totalCount
                );

                _logger.LogInformation($"Retrieved {userDtos.Count} users out of {totalCount}.");
                return OperationResult<PagedResponse<UserResponseDto>>.Success(pagedResponse);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("The operation to retrieve all users was canceled.");
                return _errorHandler.HandleCancelationToken<PagedResponse<UserResponseDto>>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all users.", ex);
                return _errorHandler.HandleException<PagedResponse<UserResponseDto>>(ex);
            }
        }
    }
}
