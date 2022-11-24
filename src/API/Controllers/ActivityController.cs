using Application.Activities;
using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Comments = Application.Comments;

namespace API.Controllers;

public class ActivitiesController : BaseController {
  [HttpGet]
  [ProducesResponseType(typeof(PaginatedList<ActivityDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<PaginatedList<ActivityDto>>> List(
    int? limit,
    int? offset,
    bool isGoing,
    bool isHost,
    DateTime? startDate,
    CancellationToken ct) => 
    Ok(await Mediator.Send(
      new List.Query(
        limit ?? 0,
        offset ?? 3,
        isGoing,
        isHost,
        startDate ?? DateTime.Now), ct));

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult> Details(Guid id, CancellationToken ct) =>
    Ok(await Mediator.Send(new Details.Query { Id = id }, ct));

  [HttpPost]
  public async Task<ActionResult> Create(Create.Command command, CancellationToken ct) =>
    this.Created(await Mediator.Send(command, ct));

  [HttpPut("{id}")]
  [Authorize(Policy = "IsHostCreatedActivity")]
  public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command, CancellationToken ct) {
    command.Id = id;
    return await Mediator.Send(command, ct);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Unit>> EditPartial(Guid id, EditPartial.Command command, CancellationToken ct) {
    command.Id = id;
    return await Mediator.Send(command, ct);
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

  [HttpPost("{id}/comment")]
  public async Task<ActionResult<Comments.CommentDto>> Comment(Guid id, [FromBody] string body, CancellationToken ct) {
    var command = new Comments.Create.Command {
      ActivityId = id,
      Body = body,
      Username = GetCurrUserName(),
    };
    return await Mediator.Send(command, ct);
  }

  private ActionResult Created(Guid id) =>
    base.Created(nameof(ActivitiesController.Details), new { Id = id });
}