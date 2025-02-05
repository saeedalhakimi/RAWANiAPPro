using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.CommandHandlers
{
    public class CreatePostCommandHandler 
        : IRequestHandler<CreatePostCommand, OperationResult<PostResponseDto>>
    {
        private readonly IAppLogger<CreatePostCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;
        private readonly IPostRepository _postRepository;
        private readonly IFileService _fileService;

        public CreatePostCommandHandler(
            IAppLogger<CreatePostCommandHandler> logger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler, 
            IPostRepository postRepository,
            IFileService fileService)
        {
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
            _postRepository = postRepository;
            _fileService = fileService;
        }

        public async Task<OperationResult<PostResponseDto>> Handle(
            CreatePostCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation($"Checking if user profile '{request.UserProfileID}' Exists..");
                var isUserExists = await _postRepository.IsUserProfileExistsAsync(request.UserProfileID, cancellationToken);

                if (!isUserExists.IsSuccess)
                {
                    var errorMessage = isUserExists.Errors.First().Message;

                    _logger.LogError("An error occurred while checking if user profile {UserProfileID} exists: {ErrorMessage}",
                        request.UserProfileID, errorMessage);

                    return OperationResult<PostResponseDto>.Failure(isUserExists.Errors);
                }

                if (!isUserExists.Data)
                {
                    var notFoundError = new Error(
                        ErrorCode.NotFound,
                        "User Profile Not Found",
                        $"User Profile '{request.UserProfileID}' does not exist."
                    );

                    _logger.LogWarning("User Profile {UserProfileID} was not found.", request.UserProfileID);

                    return OperationResult<PostResponseDto>.Failure(notFoundError);
                }
                // Step 1: Save the post image and generate a unique link
                _logger.LogInformation("Starting to save the post image.");
                string postImageLink = await SavePostImageAsync(request.PostImage);
                _logger.LogInformation("Post image saved successfully. Image link: {PostImageLink}", postImageLink);

                // Step 2: Create the Post entity
                _logger.LogInformation("Creating the Post entity.");
                var post = Post.Create(PostGuid.CreateNew().Value, request.UserProfileID,
                    request.PostTitle, request.PostContent, postImageLink);
                if (!post.IsSuccess)
                {
                    _logger.LogError("Failed to create the Post entity. Errors: {Errors}", string.Join(", ", post.Errors));
                    return OperationResult<PostResponseDto>.Failure(post.Errors); 
                }
                _logger.LogInformation("Post entity created successfully. PostID: {PostID}", post.Data!.PostID);

                // Step 3: Save the Post to the repository
                _logger.LogInformation("Saving the Post entity to the repository.");
                var result = await _postRepository.CreatePostAsync(post.Data!, cancellationToken);
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to save the Post entity to the repository. Errors: {Errors}", string.Join(", ", result.Errors));
                    return OperationResult<PostResponseDto>.Failure(post.Errors);
                }

                _logger.LogInformation("Post entity saved successfully to the repository.");

                // Step 4: Map the Post entity to a response DTO
                _logger.LogInformation("Mapping the Post entity to a response DTO.");
                var response = PostMappers.ToPostResponseDto(post.Data!);

                _logger.LogInformation("Post creation process completed successfully.");
                return OperationResult<PostResponseDto>.Success(response);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to create post  was canceled.");
                return _errorHandler.HandleCancelationToken<PostResponseDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while creating post", ex);
                return _errorHandler.HandleException<PostResponseDto>(ex);
            }
        }

        private async Task<string> SavePostImageAsync(IFormFile postImage)
        {
            if (postImage == null || postImage.Length == 0)
                return null;

            var uniqueFileName = await _fileService.SaveFileAsync(
                postImage.OpenReadStream(), postImage.FileName);
            return $"/uploads/{uniqueFileName}";
        }
    }
}
