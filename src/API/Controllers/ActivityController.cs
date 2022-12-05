using Application.Activities;
using Application.Common.Models;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Comments = Application.Comments;

namespace API.Controllers;

public class ActivitiesController : BaseController {

  [HttpGet]
  public async Task<ActionResult<PaginatedList<ActivityDto>>> GetAll(
    int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate, CancellationToken ct) =>
    Ok(await Mediator.Send(new List.Query(limit ?? 3, offset ?? 0, isGoing, isHost, startDate ?? DateTime.Now), ct));

  [HttpGet("{id}")]
  public async Task<ActionResult<ActivityDto>> Get(Guid id, CancellationToken ct) =>
    Ok(await Mediator.Send(new Details.Query(id), ct));

  [HttpPost]
  public async Task<ActionResult<Guid>> Create([FromBody] Create.Command command, CancellationToken ct) =>
    this.Created(await Mediator.Send(command, ct));

  [HttpPut("{id}")]
  [Authorize(Policy = IsHostRequirement.PolicyName)]
  public async Task<ActionResult> Edit(Guid id, Edit.Command command, CancellationToken ct) =>
    Ok(await Mediator.Send(command.SetId(id), ct));

  [HttpPatch("{id}")]
  [Authorize(Policy = IsHostRequirement.PolicyName)]
  public async Task<ActionResult<Unit>> EditPartial(Guid id, EditPartial.Command command, CancellationToken ct) =>
    Ok(await Mediator.Send(command with { Id = id }, ct));

  [HttpDelete("{id}")]
  [Authorize(Policy = IsHostRequirement.PolicyName)]
  public async Task<ActionResult> Delete(Guid id, CancellationToken ct) {
    await Mediator.Send(new Delete.Command(id), ct);
    return NoContent();
  }

  [HttpPost("{id}/attend")]
  public async Task<ActionResult> Attend(Guid id, CancellationToken ct) =>
    Ok(await Mediator.Send(new Attend.Command(id), ct));

  [HttpDelete("{id}/attend")]
  public async Task<ActionResult> Unattend(Guid id, CancellationToken ct) {
    await Mediator.Send(new Unattend.Command(id), ct);
    return NoContent();
  }

  [HttpPost("{id}/comment")]
  public async Task<ActionResult<Comments.CommentDto>> Comment(Guid id, [FromBody] string body, CancellationToken ct) =>
    await Mediator.Send(new Comments.Create.Command(id, UserId, body), ct);

  private ActionResult Created(Guid id) =>
    base.Created(nameof(ActivitiesController.Get), new { Id = id });
}