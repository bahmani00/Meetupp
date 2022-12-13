using Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UserController : BaseController {
  /// <summary>
  /// Loging using email and password
  /// </summary>
  /// <param name="query"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [AllowAnonymous]
  [HttpPost("login")]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
  public async Task<ActionResult<UserDto>> Login(Login.Query query, CancellationToken ct) {
    return Ok(await Mediator.Send(query, ct));
  }

  /// <summary>
  /// Register yourself using email and a strong password
  /// </summary>
  /// <param name="command"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [AllowAnonymous]
  [HttpPost("register")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<UserDto>> Register(Register.Command command, CancellationToken ct) {
    var res = await Mediator.Send(command, ct);
    return StatusCode(StatusCodes.Status201Created, res);
  }

  /// <summary>
  /// Get your profile info
  /// </summary>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpGet]
  [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
  public async Task<ActionResult<UserDto>> CurrentUser(CancellationToken ct) =>
    Ok(await Mediator.Send(new CurrentUser.Query(), ct));
}