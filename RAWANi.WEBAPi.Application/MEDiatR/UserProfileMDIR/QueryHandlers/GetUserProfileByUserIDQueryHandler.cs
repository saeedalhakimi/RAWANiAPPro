using MediatR;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Contracts.UserProfileDtos.Responses;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.UserProfileMDIR.Queries;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserProfileMDIR.QueryHandlers
{
    public class GetUserProfileByUserIDQueryHandler 
        : IRequestHandler<GetUserProfileByUserIDQuery
            , OperationResult<IEnumerable<UserProfileResponseDto>>>
    {
        private readonly DataContext _ctx;
        private readonly IAppLogger<GetUserProfileByUserIDQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;

        public GetUserProfileByUserIDQueryHandler(
            DataContext ctx,
            IAppLogger<GetUserProfileByUserIDQueryHandler> logger,
            ILoggMessagingService messagingService,
            ErrorHandler errorHandler)
        {
            _ctx = ctx;
            _logger = logger;
            _errorHandler = errorHandler;
            _messagingService = messagingService;
        }

        public async Task<OperationResult<IEnumerable<UserProfileResponseDto>>> Handle(
            GetUserProfileByUserIDQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Step 1: Find all user profiles for the given user ID
                _logger.LogInformation($"Retrieving user profiles for user ID: {request.UserID}...");
                var userProfiles = await _ctx.UserProfiles
                    .Include(up => up.BasicInfo) // Include basic information
                    .Where(up => up.IdentityID == request.UserID) // Filter by user ID
                    .ToListAsync(cancellationToken);

                if (userProfiles == null || !userProfiles.Any())
                {
                    _logger.LogWarning($"No user profiles found for user ID {request.UserID}.");
                    return OperationResult<IEnumerable<UserProfileResponseDto>>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User Profiles Not Found",
                        $"No user profiles found for user ID {request.UserID}."
                    ));
                }

                var userProfilesDto = userProfiles.Select(up => new UserProfileResponseDto
                {
                    UserProfileID = up.UserProfileID,
                    IdentityID = up.IdentityID,
                    FirstName = up.BasicInfo.FirstName,
                    LastName = up.BasicInfo.LastName,
                    Email = up.BasicInfo.Email,
                    DateOfBirth = up.BasicInfo.DateOfBirth,
                    Gender = up.BasicInfo.Gender,
                    ImageLink = up.ImageLink,
                    CreatedAt = up.CreatedAt,
                    LastUpdatedAt = up.LastUpdatedAt,
                }).ToList();

                _logger.LogInformation($"Retrieved {userProfilesDto.Count} user profiles for user ID {request.UserID}.");
                return OperationResult<IEnumerable<UserProfileResponseDto>>.Success(userProfilesDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to retrieve user profiles for user ID {request.UserID} was canceled.");
                return _errorHandler.HandleCancelationToken<IEnumerable<UserProfileResponseDto>>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user profiles for user ID {request.UserID}.", ex);
                return _errorHandler.HandleException<IEnumerable<UserProfileResponseDto>>(ex);
            }
        }
    }
}
