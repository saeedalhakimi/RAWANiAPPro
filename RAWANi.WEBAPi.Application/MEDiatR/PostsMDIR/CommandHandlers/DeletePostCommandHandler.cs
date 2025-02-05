using MediatR;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads;
using RAWANi.WEBAPi.Application.Models;
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
    public class DeletePostCommandHandler
        : IRequestHandler<DeletePostCommand, OperationResult<bool>>
    {
        private readonly IAppLogger<DeletePostCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;
        private readonly IPostRepository _postRepository;

        public DeletePostCommandHandler(
            IAppLogger<DeletePostCommandHandler> logger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler, 
            IPostRepository postRepository)
        {
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
            _postRepository = postRepository;
        }

        public async Task<OperationResult<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(_messagingService.GetLoggMessage(
                nameof(LoggMessage.MDIHandlingRequest)));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                //Step 1: retrieve post
                var post = await _postRepository.GetPostByPostIDAsync(request.PostID, cancellationToken);
                if (!post.IsSuccess) return OperationResult<bool>.Failure(post.Errors);



                if (post.Data?.UserProfileID != request.UserProfileID)
                {
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.ValidationError,
                        "Unauthorized Access",
                        "You do not have permission to delete this post."
                    ));
                }

                var result = await _postRepository.DeletePostAsync(
                    request.PostID, request.UserProfileID, cancellationToken);
                if(!result.IsSuccess) return OperationResult<bool>.Failure(result.Errors);

                return OperationResult<bool>.Success(result.Data);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"The operation to delete post  was canceled.");
                return _errorHandler.HandleCancelationToken<bool>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while deleting post", ex);
                return _errorHandler.HandleException<bool>(ex);
            }
        }
    }
}
