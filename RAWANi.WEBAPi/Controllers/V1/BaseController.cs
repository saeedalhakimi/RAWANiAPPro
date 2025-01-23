using Microsoft.AspNetCore.Mvc;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Models;

namespace RAWANi.WEBAPi.Controllers.V1
{
    public class BaseController<T> : Controller
    {

        private readonly IAppLogger<T> _logger;

        public BaseController(IAppLogger<T> appLogger)
        {
            _logger = appLogger;
        }

        // Dictionary for status code mappings
        private static readonly Dictionary<ErrorCode, (int StatusCode, string StatusPhrase)> StatusMappings = new()
        {
            // in use
            { ErrorCode.UnknownError, (500, "Internal Server Error")}, // Indicates an unexpected error on the server
            { ErrorCode.NotFound, (404, "Not Found")},
            { ErrorCode.DatabaseError, (503, "Database Service Unavailable")},
            { ErrorCode.InvalidInput, (400, "Bad Request")}, // Indicates that the request is invalid or malformed
            { ErrorCode.OperationCancelled, (499, "Client Closed Request")}, // Indicates that the client closed the request before the server could respond
            { ErrorCode.InternalServerError, (500, "Internal Server Error")}, // Indicates an unexpected error on the server
            { ErrorCode.ConflictError, (409, "Conflict error occurred")}, // Indicates that the request could not be completed due to a conflict
            { ErrorCode.ValidationError, (422, "Unprocessable Entity")}, // Indicates that the request could not be completed due to validation errors
            { ErrorCode.Unauthorized, (401, "Unauthorized access")}, // Indicates that the request could not be completed due to a lack of authorization
            { ErrorCode.LockedOut, (423, "Locked Out")}, // Indicates that the request could not be completed due to a lack of authorization
            { ErrorCode.BadRequest, (400, "Bad Request")}, // Indicates that the request could not be completed due to a lack of authorization
        };

        protected IActionResult HandleErrorResponse<T>
            (OperationResult<T> result)
        {
            try
            {
                if (result == null || result.Errors == null || !result.Errors.Any())
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred.");
                }

                // Log the error response
                var errorMessages = result.Errors.Select(e => e.Message).ToList();
                var errorDetails = result.Errors.Select(e => e.Details).ToList();

                _logger.LogError($"Errors occurred: {string.Join(", ", errorMessages)}. CorrelationId: {HttpContext.TraceIdentifier}");

                var errorCode = result.Errors.FirstOrDefault()?.Code ?? ErrorCode.UnknownError;
                var statusCode = StatusMappings.ContainsKey(errorCode) ? StatusMappings[errorCode].StatusCode : 500;
                var statusPhrase = StatusMappings.ContainsKey(errorCode) ? StatusMappings[errorCode].StatusPhrase : "Internal Server Error";

                // Construct the error response object
                var apiError = new ErrorResponse
                {
                    Timestamp = result.Timestamp ?? DateTime.UtcNow,
                    CorrelationId = HttpContext.TraceIdentifier,
                    Errors = errorMessages,
                    ErrorsDetails = errorDetails!,
                    StatusCode = statusCode,
                    StatusPhrase = statusPhrase,
                    Path = HttpContext.Request.Path,
                    Method = HttpContext.Request.Method,
                    Detail = $"An error occurred while processing the request. {statusPhrase}"
                };

                //stopwatch.Stop();
                //_logger.LogInformation($"Error response generated. Operation took {stopwatch.ElapsedMilliseconds}ms");

                // Return the mapped error response
                return StatusCode(apiError.StatusCode, apiError);
            }
            catch (Exception ex)
            {
                //// Catch any unexpected exceptions
                _logger.LogError($": Unexpected error occurred.", ex);
                //stopwatch.Stop();
                // _logger.LogInformation($": Exception occurred. Operation took {stopwatch.ElapsedMilliseconds}ms");

                // Return an internal server error
                var apiError = new ErrorResponse
                {
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = HttpContext.TraceIdentifier,
                    StatusCode = 500,
                    StatusPhrase = "Internal Server Error",
                    Path = HttpContext.Request.Path,
                    Method = HttpContext.Request.Method,
                    Detail = "An unexpected error occurred. Please try again later.",
                    Errors = new List<string> { ex.Message }
                };

                return StatusCode(apiError.StatusCode, apiError);
            }
        }
    }
}
