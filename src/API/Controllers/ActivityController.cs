using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ActivitiesController : BaseController {
  [HttpGet]
  [ProducesResponseType(typeof(List.ActivitiesEnvelope), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<List.ActivitiesEnvelope>> List(int? limit,
      int? offset, bool isGoing, bool isHost, DateTime? startDate, CancellationToken ct) {
    return Ok(await Mediator.Send(new List.Query(limit, offset, isGoing, isHost, startDate), ct));
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult> Details(Guid id, CancellationToken ct) =>
    Ok(await Mediator.Send(new Details.Query { Id = id }, ct));

  [HttpPost]
  public async Task<ActionResult> Create(Create.Command command, CancellationToken ct) =>
    this.Created(await Mediator.Send(command, ct));

  [HttpPut("{id}")]
  [Authorize(Policy = "IsHostCreatedActivity")]
  public async Task<ActionResult<Unit>> Edit(Guid id, ActivityDto activity, CancellationToken ct) {
    activity.Id = id;
    return await Mediator.Send(new Edit.Command { Activity = activity }, ct);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Unit>> EditPartial(Guid id, ActivityDto activity, CancellationToken ct) {
    activity.Id = id;
    return await Mediator.Send(new EditPartial.Command { Activity = activity }, ct);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "IsHostCreatedActivity")]
  public async Task<ActionResult> Delete(Guid id, CancellationToken ct) {
    await Mediator.Send(new Delete.Command { Id = id }, ct);
    return NoContent();
  }

  [HttpPost("{id}/attend")]
  public async Task<ActionResult<Unit>> Attend(Guid id, CancellationToken ct) {
    return await Mediator.Send(new Attend.Command { Id = id }, ct);
  }

  [HttpDelete("{id}/attend")]
  public async Task<ActionResult> Unattend(Guid id, CancellationToken ct) {
    await Mediator.Send(new Unattend.Command { Id = id }, ct);
    return NoContent();
  }

  private ActionResult Created(Guid id) =>
    base.Created(nameof(ActivitiesController.Details), new { Id = id });
}