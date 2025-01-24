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
        private readonly IEmailService _emailService;
        public RegisterIdentityCommandHandler(
            DataContext ctx,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAppLogger<RegisterIdentityCommandHandler> appLogger,
            ILoggMessagingService messagingService,
            JwtService jwtService,
            IFileService fileService,
            ErrorHandler errorHandler,
            IEmailService emailService)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = appLogger;
            _messagingService = messagingService;
            _jwtService = jwtService;
            _fileService = fileService;
            _errorHandler = errorHandler;
            _emailService = emailService;
        }
        
        public async Task<OperationResult<ResponseWithTokensDto>> Handle(
            RegisterIdentityCommand request, CancellationToken cancellationToken)
        {
            // Log the start of the request handling
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest), new[] { nameof(RegisterIdentityCommand) }));

            using var transaction = _ctx.Database.BeginTransaction();
            _logger.LogInformation("Transaction initiated successfully.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Step 1: Check if the user already exists
                _logger.LogInformation("Checking if user already exists with username: {Username}", request.Username);
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user != null) 
                {
                    _logger.LogWarning("User already exists with username: {Username}", request.Username);
                    await transaction.RollbackAsync(cancellationToken);
                    return _errorHandler.ResourceAlreadyExists<ResponseWithTokensDto>(request.Username);
                }

                // Step 2: Save the profile picture and generate a unique link
                _logger.LogInformation("Saving profile picture for user: {Username}", request.Username);
                string imageLink = await SaveProfilePictureAsync(request.ProfilePicture);


                // Step 3: Create the identity user
                _logger.LogInformation("Creating identity user for username: {Username}", request.Username);
                var identity = new IdentityUser
                {
                    UserName = request.Username,
                    Email = request.Username,
                };
                var identityResult = await _userManager.CreateAsync(identity, request.Password);
                if (!identityResult.Succeeded)
                {
                    _logger.LogError("Failed to create identity user: {Errors}",
                         string.Join("; ", identityResult.Errors.Select(e => e.Description)));
                    await transaction.RollbackAsync(cancellationToken);
                    var errorDescriptions = string.Join("; ", identityResult.Errors.Select(e => e.Description));
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.InternalServerError, $"Identity Creation Error: {errorDescriptions}"
                        , "An error occurred while creating the user account."));
                }

                // Step 4: Generate an email confirmation token
                _logger.LogInformation("Generating email confirmation token for user: {Username}", request.Username);
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identity);

                // Step 5: Send the confirmation email
                _logger.LogInformation("Sending confirmation email to user: {Username}", request.Username);
                var confirmationLink = $"https://yourapp.com/confirm-email?token={emailConfirmationToken}&email={request.Username}";
                var emailBody = $"Click the link to confirm your email: {confirmationLink}";

                //await _emailService.SendEmailAsync(request.Username, "Confirm Your Email", emailBody); use later for production
                _logger.LogInformation("Confirmation email sent to user: {Username}", request.Username);

                _logger.LogInformation($"EmailConToken : {confirmationLink}"); //for dev inv only

                //Step 6: Create the basic information
                _logger.LogInformation("Creating basic information for user: {Username}", request.Username);
                var basicInformation = BasicInformation.Create(
                    request.FirstName, request.LastName, request.Username, 
                    request.DateOfBirth,request.Gender);
                if (!basicInformation.IsSuccess)
                {
                    _logger.LogError("Failed to create basic information: {Errors}",
                        string.Join("; ", basicInformation.Errors.Select(e => e.Message)));
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResult<ResponseWithTokensDto>.Failure(basicInformation.Errors);
                }

                // Step 7: Create the user profile
                _logger.LogInformation("Creating user profile for user: {Username}", request.Username);
                var userProfile = UserProfile.Create(identity.Id, basicInformation.Data!, imageLink);
                if (!userProfile.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResult<ResponseWithTokensDto>.Failure(userProfile.Errors);
                }
                await _ctx.UserProfiles.AddAsync(userProfile.Data!, cancellationToken);
                await _ctx.SaveChangesAsync(cancellationToken);

                // Step 8: Add the user to the default role
                const string defaultRole = "User "; // Define the default role
                _logger.LogInformation("Adding user to default role: {Role}", defaultRole);
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    _logger.LogInformation("Creating default role: {Role}", defaultRole);
                    await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                }

                var roleResult = await _userManager.AddToRoleAsync(identity, defaultRole);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to add user to role: {Errors}",
                        string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                    await transaction.RollbackAsync(cancellationToken);
                    var roleErrorDescriptions = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    return OperationResult<ResponseWithTokensDto>.Failure(new Error(
                        ErrorCode.InternalServerError,
                        $"Role Assignment Error: {roleErrorDescriptions}",
                        "An error occurred while assigning the user role."
                    ));
                }
                _logger.LogInformation("User {Username} added to role {Role}", request.Username, defaultRole);

                // Commit the transaction
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully for user: {Username}", request.Username);

                // Step 8: Generate the JWT tokens
                _logger.LogInformation("Generating JWT tokens for user: {Username}", request.Username);
                var roles = new List<string> { defaultRole }; // Add more roles if needed
                var accessToken = _jwtService.GenerateAccessToken(identity, userProfile.Data!, roles);

                return OperationResult<ResponseWithTokensDto>.Success(new ResponseWithTokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = null, // Implement refresh token generation
                    Message = "Registration successful. Please check your email to confirm your account."
                });
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to create user '{request.Username}' was canceled.");
                return _errorHandler.HandleCancelationToken<ResponseWithTokensDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while registering user: {Username}", request.Username, ex);
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
