using MediatR;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.CommandHandlers;
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
    public class GetAPostCommentQueryHandler
        : IRequestHandler<GetAPostCommentQuery, OperationResult<PostWithaSelectedCommentResponseDto>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<GetAPostCommentQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetAPostCommentQueryHandler(IPostRepository postRepository, IAppLogger<GetAPostCommentQueryHandler> logger, ILoggMessagingService messagingService, IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PostWithaSelectedCommentResponseDto>> Handle(GetAPostCommentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postRepository.GetPostByPostIDAsync(
                    request.PostId, cancellationToken);
                if (!post.IsSuccess)
                    return OperationResult<PostWithaSelectedCommentResponseDto>
                        .Failure(post.Errors);

                var comment = await _postRepository.GetCommentByIDAsync(
                    request.CommentId, cancellationToken);
                if (!comment.IsSuccess)
                    return OperationResult<PostWithaSelectedCommentResponseDto>
                        .Failure(comment.Errors);

                var postResult = PostMappers.ToPostResponseDto(post.Data!);
                var commentResult = PostMappers.ToPostCommentResponseDto(comment.Data!);

                var response = new PostWithaSelectedCommentResponseDto
                {
                    Post = postResult,
                    SelectedComment = commentResult
                };

                return OperationResult<PostWithaSelectedCommentResponseDto>.Success(response);
            }
            catch(OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<PostWithaSelectedCommentResponseDto>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<PostWithaSelectedCommentResponseDto>(ex);
            }
        }
    }
}
