using Application.Followers;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/profiles")]
public class FollowersController : BaseController {

  [HttpPost("{userId}/follow")]
  public async Task<ActionResult> Follow(string userId, CancellationToken ct) =>
    Ok(await Mediator.Send(new Add.Command(userId), ct));

  [HttpDelete("{userId}/follow")]
  public async Task<ActionResult> Unfollow(string userId, CancellationToken ct) {
    await Mediator.Send(new Delete.Command(userId), ct);
    return NoContent();
  }

  [HttpGet("{userId}/follow")]
  public async Task<ActionResult<List<Profile>>> GetFollowings(string userId, string predicate, CancellationToken ct) =>
    await Mediator.Send(new List.Query(userId, predicate), ct);
}