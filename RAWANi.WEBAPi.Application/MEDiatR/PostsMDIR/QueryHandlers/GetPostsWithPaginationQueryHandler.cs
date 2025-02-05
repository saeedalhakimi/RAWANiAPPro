using MediatR;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Contracts.PostDtos;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Queries;
using RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.QueryHandlers;
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
    public class GetPostsWithPaginationQueryHandler :
        IRequestHandler<GetPostsWithPaginationQuery, 
            OperationResult<PagedResponse<PostResponseDto>>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppLogger<GetAllUsersQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetPostsWithPaginationQueryHandler(
            IPostRepository postRepository, 
            IAppLogger<GetAllUsersQueryHandler> logger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _logger = logger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PagedResponse<PostResponseDto>>> Handle(
            GetPostsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var postTotalCount = await _postRepository.GetpostCountAsync(
                    request.UserProfile, cancellationToken);
                if (!postTotalCount.IsSuccess) return OperationResult<PagedResponse<
                    PostResponseDto>>.Failure(postTotalCount.Errors);

                var posts = await _postRepository.GetPostsWithPaginationAsync(
                    request.UserProfile, request.PageNumber, request.PageSize,
                    request.SortColumn, request.SortDirection, cancellationToken);
                if (!posts.IsSuccess) return OperationResult<PagedResponse<
                    PostResponseDto>>.Failure(posts.Errors);

                var postCount = posts.Data?.ToList().Count ?? 0;
                var postDtos = posts.Data?.Select(PostMappers.ToPostResponseDto).ToList() ?? new List<PostResponseDto>();

                var response = new PagedResponse<PostResponseDto>(
                    postDtos, request.PageNumber, request.PageSize, postTotalCount.Data, postCount);

                _logger.LogInformation($"Retrieved {postCount} posts out of {postTotalCount}.");
                return OperationResult<PagedResponse< PostResponseDto>>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("The operation to retrieve all users was canceled.");
                return _errorHandler.HandleCancelationToken<PagedResponse<PostResponseDto>>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all users.", ex);
                return _errorHandler.HandleException<PagedResponse<PostResponseDto>>(ex);
            }
        }
    }
}
