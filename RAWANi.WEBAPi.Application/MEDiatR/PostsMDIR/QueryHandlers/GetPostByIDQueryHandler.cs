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
    public class GetPostByIDQueryHandler
        : IRequestHandler<GetPostByIDQuery, OperationResult<PostResponseDto>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<GetPostByIDQueryHandler> _appLogger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetPostByIDQueryHandler(
            IPostRepository postRepository, 
            IAppLogger<GetPostByIDQueryHandler> appLogger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _appLogger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PostResponseDto>> Handle(
            GetPostByIDQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postRepository.GetPostByPostIDAsync(
                    request.PostID, cancellationToken);
                if (!post.IsSuccess) return OperationResult<PostResponseDto>.Failure(post.Errors);

                var response = PostMappers.ToPostResponseDto(post.Data!);
                return OperationResult<PostResponseDto>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandler.HandleCancelationToken<PostResponseDto>(ex);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException<PostResponseDto>(ex);  
            }
        }
    }
}
