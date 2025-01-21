using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class RegisterIdentityCommandHandler 
        : IRequestHandler<RegisterIdentityCommand, OperationResult<ResponseWithTokensDto>>
    {
        private readonly DataContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLogger<RegisterIdentityCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly JwtService _jwtService;
        private readonly IFileService _fileService;
        private readonly ErrorHandler _errorHandler;
        public RegisterIdentityCommandHandler(
            DataContext ctx,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAppLogger<RegisterIdentityCommandHandler> appLogger,
            ILoggMessagingService messagingService,
            JwtService jwtService,
            IFileService fileService,
            ErrorHandler errorHandler)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = appLogger;
            _messagingService = messagingService;
            _jwtService = jwtService;
            _fileService = fileService;
            _errorHandler = errorHandler;
        }
        
        public async Task<OperationResult<ResponseWithTokensDto>> Handle(
            RegisterIdentityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest)));

            using var transaction = _ctx.Database.BeginTransaction();
            _logger.LogInformation("Transaction initiated successfully.");

            try
            {
                // Step 1: Check if the user already exists
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user != null) 
                { 
                    await transaction.RollbackAsync(cancellationToken);
                    return _errorHandler.ResourceAlreadyExists<ResponseWithTokensDto>(request.Username);
                }

                // Step 2: Save the profile picture and generate a unique link
                string imageLink = await SaveProfilePictureAsync(request.ProfilePicture);


                // Step 3: Create the identity user
                var identity = new IdentityUser
                {
                    UserName = request.Username,
                    Email = request.Username,
                };
                var identityResult = await _userManager.CreateAsync(identity, request.Password);
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    var errorDescriptions = string.Join("; ", identityResult.Errors.Select(e => e.Description));
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.InternalServerError, $"Identity Creation Error: {errorDescriptions}"
                        , "An error occurred while creating the user account."));
                }
                _logger.LogInformation($"User {identity.UserName} created successfully.");

                //Step 3: Create the basic information
                var basicIformation = BasicInformation.Create(
                    request.FirstName, request.LastName, request.Username, 
                    request.DateOfBirth,request.Gender);
                if (!basicIformation.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResult<ResponseWithTokensDto>.Failure(basicIformation.Errors);
                }

                // Step 4: Create the user profile
                var userProfile = UserProfile.Create(identity.Id, basicIformation.Payload!, imageLink);
                if (!userProfile.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResult<ResponseWithTokensDto>.Failure(userProfile.Errors);
                }
                _ctx.UserProfiles.Add(userProfile.Payload!);
                await _ctx.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"User profile {userProfile.Payload!.UserProfileID} created successfully.");

                // Step 5: Add the user to the default role
                const string defaultRole = "User "; // Define the default role
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                }

                var roleResult = await _userManager.AddToRoleAsync(identity, defaultRole);
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    var roleErrorDescriptions = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        $"Role Assignment Error: {roleErrorDescriptions}",
                        "An error occurred while assigning the user role."
                    ));
                }
                _logger.LogInformation($"User {identity.UserName} added to the {defaultRole} role.");

                // Commit the transaction
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully.");

                // Step 8: Generate the JWT tokens
                var roles = new List<string> { defaultRole }; // Add more roles if needed
                var accessToken = _jwtService.GenerateAccessToken(identity, userProfile.Payload!, roles);

                return OperationResult<ResponseWithTokensDto>.Success(new ResponseWithTokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = null // Implement refresh token generation
                });


            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return _errorHandler.HandleException<ResponseWithTokensDto>(ex);
            }
        }

        private async Task<string> SaveProfilePictureAsync(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
                return null;

            var uniqueFileName = await _fileService.SaveFileAsync(
                profilePicture.OpenReadStream(), profilePicture.FileName);
            return $"/uploads/{uniqueFileName}";
        }

       
    }
}
