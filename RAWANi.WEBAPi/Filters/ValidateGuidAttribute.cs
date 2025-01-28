using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RAWANi.WEBAPi.Models;

namespace RAWANi.WEBAPi.Filters
{
    public class ValidateGuidAttribute : ActionFilterAttribute
    {
        private readonly string _key;


        public ValidateGuidAttribute(string key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key), "Key cannot be null");

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Try to get the value from the action arguments
            if (!context.ActionArguments.TryGetValue(_key, out var value))
            {
                // Optional: Handle the case where the key is missing
                context.Result = new BadRequestObjectResult(new ErrorResponse
                {
                    StatusCode = 400,
                    StatusPhrase = "Bad Request",
                    Timestamp = DateTime.UtcNow,
                    Errors = { $": The parameter '{_key}' is required." },
                    Path = context.HttpContext.Request.Path,  // Capture the request path
                    Method = context.HttpContext.Request.Method,  // Capture the request method (e.g., GET, POST)
                    Detail = "The profile ID is missing.",
                    CorrelationId = context.HttpContext.TraceIdentifier // Capture the CorrelationId

                });

                return;
            }

            // Validate if the value is a valid Guid
            if (Guid.TryParse(value?.ToString(), out var guid))
            {
                return;
            }

            // If invalid, create an error response
            var apiError = new ErrorResponse
            {
                StatusCode = 400,
                StatusPhrase = "Bad Request",
                Timestamp = DateTime.UtcNow,
                Path = context.HttpContext.Request.Path,  // Capture the request path
                Method = context.HttpContext.Request.Method,  // Capture the request method (e.g., GET, POST)
                Detail = "The provided user profile id is not a valid Guid.",
                CorrelationId = context.HttpContext.TraceIdentifier // Capture the CorrelationId
            };

            apiError.Errors.Add($": The value of '{_key}' is not a valid Guid format.");
            context.Result = new ObjectResult(apiError) { StatusCode = 400 };
        }
    }
}
