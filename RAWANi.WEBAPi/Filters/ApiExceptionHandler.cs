using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Models;

namespace RAWANi.WEBAPi.Filters
{
    public class ApiExceptionHandler : ExceptionFilterAttribute
    {
        //private readonly IAppLogger<AppExceptionHandler> _logger;


        //// Constructor to inject the logger
        //public AppExceptionHandler(IAppLogger<AppExceptionHandler> logger)
        //{
        //    _logger = logger;

        //}
        public override void OnException(ExceptionContext context)
        {
            // Log the exception using Serilog through the injected logger
            //_logger.LogError($": Unhandled exception occurred. Request: " +
            //$"{context.HttpContext.Request.Method} " +
            //$"{context.HttpContext.Request.Path}. Exception: " +
            //$"{context.Exception.Message}", context.Exception);

            // Use a generic error code and message
            var error = new Error(
                ErrorCode.InternalServerError,  // Always using a generic internal server error code
                $" An internal server error occurred.",
                context.Exception.Message      // The exception message as the detail
            );

            var apiError = new ErrorResponse
            {
                StatusCode = 500,
                StatusPhrase = "Internal Server Error",
                Timestamp = DateTime.UtcNow,
                Errors = { context.Exception.Message },
                Path = context.HttpContext.Request.Path,
                Method = context.HttpContext.Request.Method,
                Detail = context.Exception.Message,
                CorrelationId = context.HttpContext.TraceIdentifier

            };

            context.Result = new JsonResult(apiError) { StatusCode = 500 };
        }
    }
}
