using MediatR;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Queries;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.QueryHandlers
{
    public class GetCommentByIDQueryHandler
        : IRequestHandler<GetCommentByIDQuery, OperationResult<PostCommentResponseDto>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<GetCommentByIDQueryHandler> _appLogger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetCommentByIDQueryHandler(IPostRepository postRepository, IAppLogger<GetCommentByIDQueryHandler> appLogger, ILoggMessagingService messagingService, IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _appLogger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PostCommentResponseDto>> Handle(GetCommentByIDQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var commnet = await _postRepository.GetCommentByIDAsync(
                    request.CommentID, cancellationToken);
                if (!commnet.IsSuccess)
                    return OperationResult<PostCommentResponseDto>
                        .Failure(commnet.Errors);

                var commentDto = PostMappers.ToPostCommentResponseDto(commnet.Data!);
                return OperationResult<PostCommentResponseDto>
                    .Success(commentDto);
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
