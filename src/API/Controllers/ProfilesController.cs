using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProfilesController : BaseController {

  [HttpGet("{username}")] //username as Route parameter
  public async Task<ActionResult<Profile>> Get(string username, CancellationToken ct) {
    return await Mediator.Send(new Details.Query { Username = username }, ct);
  }

  [HttpPut]
  public async Task<ActionResult<Unit>> Edit(Edit.Command command, CancellationToken ct) {
    return await Mediator.Send(command, ct);
  }

  [HttpGet("{username}/activities")]
  public async Task<ActionResult<List<UserActivityDto>>> GetUserActivities(string username, string predicate, CancellationToken ct) {
    return await Mediator.Send(new ListActivities.Query { Username = username, Predicate = predicate }, ct);
  }
}