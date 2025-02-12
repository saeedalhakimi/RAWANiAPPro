using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads;
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
    public class CreatePostCommentCommandHandler 
        : IRequestHandler<CreatePostCommentCommand, OperationResult<PostCommentResponseDto>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<CreatePostCommentCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public CreatePostCommentCommandHandler(IPostRepository postRepository, IAppLogger<CreatePostCommentCommandHandler> logger, ILoggMessagingService messagingService, IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PostCommentResponseDto>> Handle(
            CreatePostCommentCommand request, CancellationToken cancellationToken)
        {
            
            try
            {
                //Step 1: Check if the post exists.
                var postResult = await _postRepository.IsPostExistsAsync(request.PostID, cancellationToken);
                if (!postResult.IsSuccess) 
                    return OperationResult<PostCommentResponseDto>.Failure(postResult.Errors);
                
                if (postResult.Data.Equals(false))
                {
                    return OperationResult<PostCommentResponseDto>.Failure(new Error(
                        ErrorCode.NotFound,
                        "NOT FOUND,",
                        "post not found."));
                }

                //Step 2: Create the comment.
                var newCommant = PostComment.Create(
                    Guid.NewGuid(),
                    request.PostID,
                    request.UserProfileID,
                    request.CommentContent);

                if (!newCommant.IsSuccess) return OperationResult<PostCommentResponseDto>
                        .Failure(newCommant.Errors);

                //Step 3: Save to Database.
                var result = await _postRepository.CreatePostCommentAsync(newCommant.Data!, cancellationToken);
                if (!result.IsSuccess) return OperationResult<PostCommentResponseDto>
                        .Failure(result.Errors);

                //Step 4: Map the response and return the result.
                var response = PostMappers.ToPostCommentResponseDto(newCommant.Data!);
                return OperationResult<PostCommentResponseDto>.Success(response);

            }
            catch (OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<PostCommentResponseDto>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<PostCommentResponseDto>(ex);
            }
        }
    }
}
