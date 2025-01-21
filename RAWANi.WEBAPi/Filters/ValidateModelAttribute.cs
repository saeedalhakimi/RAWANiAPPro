using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RAWANi.WEBAPi.Models;

namespace RAWANi.WEBAPi.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // Check if the ModelState has any validation errors
            if (!context.ModelState.IsValid)
            {
                // Create a custom error response
                var apiError = new ErrorResponse
                {
                    StatusCode = 400,
                    StatusPhrase = "Bad Request",
                    Timestamp = DateTime.Now,
                    Path = context.HttpContext.Request.Path,  // Capture the request path
                    Method = context.HttpContext.Request.Method,  // Capture the request method (e.g., GET, POST)
                    Detail = $"An error occurred while processing the request.",
                    CorrelationId = context.HttpContext.TraceIdentifier // Capture the CorrelationId

                };

                // Extract each error message from ModelState and add it to the Errors list
                foreach (var error in context.ModelState.Values.SelectMany(v => v.Errors))
                {
                    apiError.Errors.Add(error.ErrorMessage);
                }

                // Set the response to a 400 Bad Request with the custom error response
                context.Result = new BadRequestObjectResult(apiError);
            }
        }
    }
}
