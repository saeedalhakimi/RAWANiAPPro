using MediatR;
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
    public class UpdatePostCommentCommandHandler
        : IRequestHandler<UpdatePostCommentCommand, OperationResult<bool>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<UpdatePostCommentCommandHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public UpdatePostCommentCommandHandler(IPostRepository postRepository, IAppLogger<UpdatePostCommentCommandHandler> logger, ILoggMessagingService messagingService, IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<bool>> Handle(UpdatePostCommentCommand request, CancellationToken cancellationToken)
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
                        "You do not have permission to update this comment."
                    ));
                }

                var updateComment = comment.Data.UpdateComment(request.CommentContent);
                if (!updateComment.IsSuccess) return OperationResult<bool>.Failure(updateComment.Errors);

                var result = await _postRepository.UpdateCommentContentsAsync(comment.Data, cancellationToken);
                if (!result.IsSuccess) return OperationResult<bool>.Failure(result.Errors);

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
