using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Models
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IAppLogger<ErrorHandler> _logger;
        private readonly ILoggMessagingService _messagingService;

        public ErrorHandler(
            IAppLogger<ErrorHandler> logger,
            ILoggMessagingService messagingService)
        {
            _logger = logger;
            _messagingService = messagingService;
        }

        public OperationResult<T> ResourceAlreadyExists<T>(string key)
        {
            _logger.LogError(_messagingService.GetErrorMessage(nameof(ErrorMessage.ResourceAlreadyExists)));
            return OperationResult<T>.Failure(new Error(ErrorCode.ConflictError,
                _messagingService.GetErrorMessage(nameof(ErrorMessage.CONFLICTERROR)),
                $"{key} - {_messagingService.GetErrorMessage( nameof(ErrorMessage.ResourceAlreadyExists))}"));
        }

        public OperationResult<T> ResourceCreationFailed<T>()
        {
            _logger.LogError(_messagingService.GetErrorMessage(nameof(ErrorMessage.CreationFailed)));
            return OperationResult<T>.Failure(new Error(ErrorCode.InternalServerError,
                _messagingService.GetErrorMessage(nameof(ErrorMessage.CreationFailed)),
                _messagingService.GetErrorMessage(nameof(ErrorMessage.CreationFailed))));
        }

        public OperationResult<T> HandleException<T>(Exception ex)
        {
            _logger.LogError(_messagingService.GetErrorMessage(nameof(ErrorMessage.UnknownError)), ex);
            return OperationResult<T>.Failure(new Error(
                ErrorCode.UnknownError,
                $"An error occurred: {ex.Message}",
                $"{ex.Source} - {ex.ToString()}."));
        }

        public OperationResult<T> HandleCancelationToken<T>(OperationCanceledException ex)
        {
            _logger.LogError(_messagingService.GetErrorMessage(nameof(ErrorMessage.UnknownError)), ex);
            return OperationResult<T>.Failure(new Error(
                ErrorCode.OperationCancelled,
                $"The operation was canceled: {ex.Message}",
                $"{ex.Source} - {ex.ToString()}."));
        }
        
    }
    
}
