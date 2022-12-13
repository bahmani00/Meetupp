using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
public class ProfilesController : BaseController {

  [HttpGet("{userId}")] //userId as Route parameter
  [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
  public async Task<ActionResult<Profile>> Get(string userId, CancellationToken ct) =>
    Ok(await Mediator.Send(new Details.Query(userId), ct));

  [HttpPut]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  public async Task<ActionResult> Edit(Edit.Command command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return Ok();
  }

  [HttpGet("{userId}/activities")]
  [ProducesResponseType(typeof(List<UserActivityDto>), StatusCodes.Status200OK)]
  public async Task<ActionResult<List<UserActivityDto>>> GetUserActivities(string userId, string predicate, CancellationToken ct) =>
    Ok(await Mediator.Send(new ListActivities.Query(userId, predicate), ct));
}