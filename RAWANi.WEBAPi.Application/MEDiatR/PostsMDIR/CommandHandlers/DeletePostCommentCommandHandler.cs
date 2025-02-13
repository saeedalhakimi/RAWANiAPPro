using MediatR;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.CommandHandlers
{
    public class DeletePostCommentCommandHandler
        : IRequestHandler<DeletePostCommentCommand, OperationResult<bool>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<DeletePostCommentCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public DeletePostCommentCommandHandler(IPostRepository postRepository, IAppLogger<DeletePostCommentCommandHandler> logger, ILoggMessagingService messagingService, IErrorHandler errorHandlerService)
        {
            _postRepository = postRepository;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandlerService;
        }

        public async Task<OperationResult<bool>> Handle(DeletePostCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _postRepository.GetCommentByIDAsync(
                    request.CommentID, cancellationToken);
                if (!comment.IsSuccess) 
                    return OperationResult<bool>.Failure(comment.Errors);

                if (comment.Data?.UserProfileID != request.UserProfileID)
                {
                    return OperationResult<bool>.Failure(new Error(
                        ErrorCode.ValidationError,
                        "Unauthorized Access",
                        "You do not have permission to delete this comment."
                    ));
                }

                var result = await _postRepository.DeleteCommentAsync(
                    request.CommentID, request.UserProfileID, cancellationToken);
                if (!result.IsSuccess)
                    return OperationResult<bool>.Failure(result.Errors);

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
    }
}
