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
        private readonly IAppLogger<GetPostByIDQueryHandler> _logger;
        private readonly ILoggMessagingService _messagingService;
        private readonly IErrorHandler _errorHandler;

        public GetPostByIDQueryHandler(
            IPostRepository postRepository, 
            IAppLogger<GetPostByIDQueryHandler> appLogger, 
            ILoggMessagingService messagingService, 
            IErrorHandler errorHandler)
        {
            _postRepository = postRepository;
            _logger = appLogger;
            _messagingService = messagingService;
            _errorHandler = errorHandler;
        }

        public async Task<OperationResult<PostResponseDto>> Handle(
            GetPostByIDQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling request to retrieve post with ID: {request.PostID}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token verified. Proceeding with post retrieval.");

                _logger.LogInformation($"Fetching post from repository for PostID: {request.PostID}");
                var post = await _postRepository.GetPostByPostIDAsync(
                    request.PostID, cancellationToken);
                if (!post.IsSuccess) 
                {
                    _logger.LogWarning($"Post retrieval failed for PostID: {request.PostID}. " +
                        $"Reason: {string.Join(", ", post.Errors.Select(e => e.Message))}");
                    return OperationResult<PostResponseDto>.Failure(post.Errors); 
                }

                var response = PostMappers.ToPostResponseDto(post.Data!);

                _logger.LogInformation($"Successfully retrieved post with ID: {request.PostID}");
                return OperationResult<PostResponseDto>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"Request to retrieve post with ID: {request.PostID} was canceled.");
                return _errorHandler.HandleCancelationToken<PostResponseDto>(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while retrieving post with ID: {request.PostID}", ex);
                return _errorHandler.HandleException<PostResponseDto>(ex);  
            }
        }
    }
}
