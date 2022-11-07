using System.Threading;
using System.Threading.Tasks;
using Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UserController : BaseController {
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(Login.Query query, CancellationToken ct) {
        return await Mediator.Send(query, ct);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(Register.Command command, CancellationToken ct) {
        return await Mediator.Send(command, ct);
    }

    [HttpGet]
    public async Task<ActionResult<User>> CurrentUser(CancellationToken ct) {
        return await Mediator.Send(new CurrentUser.Query(), ct);
    }
}