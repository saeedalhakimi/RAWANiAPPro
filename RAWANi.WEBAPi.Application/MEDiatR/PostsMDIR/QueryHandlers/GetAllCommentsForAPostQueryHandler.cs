using MediatR;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Queries;
using RAWANi.WEBAPi.Application.Models;
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
    public class GetAllCommentsForAPostQueryHandler
        : IRequestHandler<GetAllCommentsForAPostQuery, OperationResult<PagedResponse<PostCommentResponseDto>>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<GetAllCommentsForAPostQueryHandler> _appLogger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetAllCommentsForAPostQueryHandler(IPostRepository postRepository, IAppLogger<GetAllCommentsForAPostQueryHandler> appLogger, ILoggMessagingService messagingService, IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _appLogger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PagedResponse<PostCommentResponseDto>>> Handle(GetAllCommentsForAPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postRepository.GetPostByPostIDAsync(request.PostID, cancellationToken);
                if (!post.IsSuccess) 
                    return OperationResult<PagedResponse<PostCommentResponseDto>>
                        .Failure(post.Errors);

                var commentsTotalCount = await _postRepository.GetPostCommentsCountAsync(
                    request.PostID, cancellationToken);
                if (!commentsTotalCount.IsSuccess)
                    return OperationResult<PagedResponse<PostCommentResponseDto>>
                        .Failure(commentsTotalCount.Errors);

                var comments = await _postRepository.GetCommentsForPostAsync(
                    request.PostID, request.PageNumber, request.PageSize,
                    request.SortColumn, request.SortDirection, cancellationToken);
                if (!comments.IsSuccess)
                    return OperationResult<PagedResponse<PostCommentResponseDto>>
                        .Failure(comments.Errors);

                var commentsCount = comments.Data?.ToList().Count ?? 0;
                var commentDtos = comments.Data?.Select(PostMappers.ToPostCommentResponseDto).ToList() ?? new List<PostCommentResponseDto>();

                var response = new PagedResponse<PostCommentResponseDto>(
                    commentDtos ,request.PageNumber, request.PageSize, commentsTotalCount.Data ,commentsCount);

                return OperationResult<PagedResponse<PostCommentResponseDto>>.Success(response);

            }
            catch (OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<PagedResponse<PostCommentResponseDto>>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<PagedResponse<PostCommentResponseDto>>(ex);    
            }
        }
    }
}
