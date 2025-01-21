using RAWANi.WEBAPi.Models;
using System.Net;

namespace RAWANi.WEBAPi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                //Log.Error(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                StatusPhrase = "Internal Server Error",
                Errors = new List<string> { exception.Message },
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path,
                Method = context.Request.Method,
                Detail = "An unexpected error occurred. Please try again later.",
                CorrelationId = context.TraceIdentifier
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
