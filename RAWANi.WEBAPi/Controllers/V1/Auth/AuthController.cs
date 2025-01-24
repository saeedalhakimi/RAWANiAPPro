using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Requests;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Register(
            [FromForm] RegisterIdentityCommand command,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering new user.");
            var result = await _mediator.Send(command, cancellationToken);
            if(!result.IsSuccess) return HandleErrorResponse(result);

            _logger.LogInformation("User registered successfully.");
            return CreatedAtRoute("Register", result);
        }

        [HttpPost(ApiRoutes.AuthRouts.Login, Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto loginDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging in user.");

            var command = new LoginCommand
            {
                Username = loginDto.Username,
                Password = loginDto.Password
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("User logged in successfully.");
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AuthRouts.RefreshToken, Name = "RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenDto refreshTokenDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refreshing token.");
            var command = new RefreshTokenCommand
            {
                RefreshToken = refreshTokenDto.RefreshToken
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("Token refreshed successfully.");
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AuthRouts.Logout, Name = "Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> Logout(
            [FromBody] RefreshTokenDto refreshTokenDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging out user.");
            var command = new LogoutCommand
            {
                RefreshToken = refreshTokenDto.RefreshToken
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return HandleErrorResponse(result);
            _logger.LogInformation("User logged out successfully.");
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AuthRouts.ForgotPassword, Name = "ForgotPassword")]
        [ValidateModel]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordCommand command, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AuthRouts.ResetPassword, Name = "ResetPassword")]
        [ValidateModel]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost(ApiRoutes.AuthRouts.ConfirmEmail, Name = "ConfirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> ConfirmEmail(
            [FromBody] ConfirmEmailCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
