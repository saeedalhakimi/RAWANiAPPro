using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Filters;

namespace RAWANi.WEBAPi.Controllers.V1.Auth
{
    [ApiVersion("1")]
    [Route(ApiRoutes.BaseRoute)]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class AuthController : BaseController<AuthController>
    {
        private readonly IMediator _mediator;
        private readonly IAppLogger<AuthController> _logger;
        public AuthController(IMediator mediator,
            IAppLogger<AuthController> appLogger) : base(appLogger)
        {
            _mediator = mediator;
            _logger = appLogger;
        }

        [HttpPost(ApiRoutes.AuthRouts.Registration, Name = "Register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromForm] RegisterIdentityCommand command)
        {
            _logger.LogInformation("Registering new user.");
            var result = await _mediator.Send(command);
            if(!result.IsSuccess) return HandleErrorResponse(result);

            _logger.LogInformation("User registered successfully.");
            return Ok(result);
        }
    }
}
