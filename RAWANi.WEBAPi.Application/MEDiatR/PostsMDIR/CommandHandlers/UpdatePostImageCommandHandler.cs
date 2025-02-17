using MediatR;
using Microsoft.AspNetCore.Http;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.QueryHandlers;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.CommandHandlers
{
    public class UpdatePostImageCommandHandler : IRequestHandler<UpdatePostImageCommand, OperationResult<bool>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<UpdatePostImageCommandHandler> _appLogger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;
        private readonly IFileService _fileService;

        public UpdatePostImageCommandHandler(
            IPostRepository postRepository, 
            IAppLogger<UpdatePostImageCommandHandler> appLogger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler, IFileService fileService)
        {
            _postRepository = postRepository;
            _appLogger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
            _fileService = fileService;
        }

        public async Task<OperationResult<bool>> Handle(UpdatePostImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var postResult = await _postRepository.GetPostByPostIDAsync(
                   request.PostID, cancellationToken);
                if (!postResult.IsSuccess) 
                    return OperationResult<bool>.Failure(postResult.Errors);

                var post = postResult.Data;
                if (post!.UserProfileID.Value != request.UserProfileID)
                {
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.ValidationError,
                        "Unauthorized Access",
                        "You do not have permission to update this post."
                    ));
                }

                var oldPostImageLink = post.ImageLink;


                // 2. Save the new image
                var newPostImageLink = await SavePostImageAsync(request.PostImage);
                if (string.IsNullOrEmpty(newPostImageLink))
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.InvalidInput,
                        "Invalid Input",
                        "Failed to save the new image"
                    ));


                post.UpdatePostImage(newPostImageLink);

                var result = await _postRepository.UpdatePostImageAsync(post, cancellationToken);
                if (!result.IsSuccess)
                {
                    // Rollback: If DB update fails, delete the newly uploaded image
                    await DeleteImageIfExistsAsync(newPostImageLink);
                    return OperationResult<bool>.Failure(result.Errors);
                } 
                    

                // Delete the old image if the update was successful
                await DeleteImageIfExistsAsync(oldPostImageLink);

                return OperationResult<bool>.Success(result.Data);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<bool>(ex);
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

        private async Task DeleteImageIfExistsAsync(string oldImageLink)
        {
            if (!string.IsNullOrEmpty(oldImageLink))
            {
                await _fileService.DeleteFileAsync(oldImageLink);
            }
        }
    }
}
