using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProfilesController : BaseController {

  [HttpGet("{username}")] //username as Route parameter
  public async Task<ActionResult<Profile>> Get(string username, CancellationToken ct) =>
    await Mediator.Send(new Details.Query(username), ct);

  [HttpPut]
  public async Task<ActionResult> Edit(Edit.Command command, CancellationToken ct) =>
    Ok(await Mediator.Send(command, ct));

  [HttpGet("{username}/activities")]
  public async Task<ActionResult<List<UserActivityDto>>> GetUserActivities(string username, string predicate, CancellationToken ct) =>
    await Mediator.Send(new ListActivities.Query(username, predicate), ct);
}