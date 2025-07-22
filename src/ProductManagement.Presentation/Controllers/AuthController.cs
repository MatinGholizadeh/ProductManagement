using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProdManagement.Application.Features.Users.Login;
using ProdManagement.Application.Features.Users.Register;

namespace ProdManagement.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    #region Constructor & DI

    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Login

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { Token = token });
    }

    #endregion Login - End

    #region Register

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] RegisterCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { Token = token });
    }

    #endregion Register - End
}
