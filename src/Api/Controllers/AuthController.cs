using MediatR;
using Microsoft.AspNetCore.Mvc;
using OaigLoan.Api.Contracts.Auth;
using OaigLoan.Application.Users.Commands.LoginUser;
using OaigLoan.Application.Users.Commands.RegisterUser;

namespace OaigLoan.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterUserCommand(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Password),
            cancellationToken);

        return Ok(ToResponse(result));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password), cancellationToken);
        return Ok(ToResponse(result));
    }

    private static AuthResponse ToResponse(Application.Auth.AuthResult result)
    {
        var user = result.User;
        return new AuthResponse(
            result.AccessToken,
            result.ExpiresAtUtc,
            new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.CreatedAtUtc));
    }
}
