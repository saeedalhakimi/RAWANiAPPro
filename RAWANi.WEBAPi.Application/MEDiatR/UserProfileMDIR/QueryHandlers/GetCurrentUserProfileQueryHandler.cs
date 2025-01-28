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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserProfileMDIR.QueryHandlers
{
    public class GetCurrentUserProfileQueryHandler
        : IRequestHandler<GetCurrentUserProfileQuery, OperationResult<UserProfileResponseDto>>
    {
        private readonly DataContext _ctx;
        private readonly IAppLogger<GetCurrentUserProfileQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly ErrorHandler _errorHandler;

        public GetCurrentUserProfileQueryHandler(
            DataContext ctx, 
            IAppLogger<GetCurrentUserProfileQueryHandler> logger, 
            ILoggMessagingService messagingService, 
            ErrorHandler errorHandler)
        {
            _ctx = ctx;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<UserProfileResponseDto>> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Find the user profile
                _logger.LogInformation($"Retrieving user profile: {request.UserProfileID}...");
                var userProfile = await _ctx.UserProfiles
                    .Include(up => up.BasicInfo) // Include basic information
                    .FirstOrDefaultAsync(up => up.UserProfileID == request.UserProfileID, cancellationToken);

                if (userProfile == null)
                {
                    _logger.LogWarning($"User profile with ID {request.UserProfileID} not found.");
                    return OperationResult<UserProfileResponseDto>.Failure(new Error(
                        ErrorCode.NotFound,
                        "User Profile Not Found",
                        $"User profile with ID {request.UserProfileID} does not exist."
                    ));
                }

                // Step 2: Map the user profile to a DTO
                var userProfileDto = new UserProfileResponseDto
                {
                    UserProfileID = userProfile.UserProfileID,
                    IdentityID = userProfile.IdentityID,
                    FirstName = userProfile.BasicInfo.FirstName,
                    LastName = userProfile.BasicInfo.LastName,
                    Email = userProfile.BasicInfo.Email,
                    DateOfBirth = userProfile.BasicInfo.DateOfBirth,
                    Gender = userProfile.BasicInfo.Gender,
                    ImageLink = userProfile.ImageLink,
                    CreatedAt = userProfile.CreatedAt,
                    LastUpdatedAt = userProfile.LastUpdatedAt
                };

                _logger.LogInformation($"User profile with ID {request.UserProfileID} retrieved successfully.");
                return OperationResult<UserProfileResponseDto>.Success(userProfileDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to retrieve user profile {request.UserProfileID} was canceled.");
                return _errorHandler.HandleCancelationToken<UserProfileResponseDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user profile {request.UserProfileID}.", ex);
                return _errorHandler.HandleException<UserProfileResponseDto>(ex);
            }
        }
    }
}
