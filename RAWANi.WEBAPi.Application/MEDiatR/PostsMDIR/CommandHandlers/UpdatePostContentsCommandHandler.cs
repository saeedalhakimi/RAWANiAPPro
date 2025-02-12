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
    public class UpdatePostContentsCommandHandler
        : IRequestHandler<UpdatePostContentsCommand, OperationResult<bool>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<UpdatePostContentsCommandHandler> _appLogger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;
        

        public UpdatePostContentsCommandHandler(IPostRepository postRepository, IAppLogger<UpdatePostContentsCommandHandler> appLogger, ILoggMessagingService messagingService, IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _appLogger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
            
        }

        public async Task<OperationResult<bool>> Handle(UpdatePostContentsCommand request, CancellationToken cancellationToken)
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
                        "You do not have permission to update this postResult."
                    ));
                }



                var updateResult = post.UpdatePostContents(request.PostTitle, request.PostContent);
                if (!updateResult.IsSuccess) return OperationResult<bool>.Failure(updateResult.Errors);

                var updatedPost = updateResult.Data;
                var result = await _postRepository.UpdatePostContentsAsync(updatedPost!, cancellationToken);
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
