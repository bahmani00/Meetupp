using Application.Followers;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/profiles")]
public class FollowersController : BaseController {
  [HttpPost("{username}/follow")]
  public async Task<ActionResult<Unit>> Follow(string username, CancellationToken ct) {
    return await Mediator.Send(new Add.Command { Username = username }, ct);
  }

  [HttpDelete("{username}/follow")]
  public async Task<ActionResult<Unit>> Unfollow(string username, CancellationToken ct) {
    return await Mediator.Send(new Delete.Command { Username = username }, ct);
  }

  [HttpGet("{username}/follow")]
  public async Task<ActionResult<List<Profile>>> GetFollowings(string username, string predicate, CancellationToken ct) {
    return await Mediator.Send(new List.Query { Username = username, Predicate = predicate }, ct);
  }
}