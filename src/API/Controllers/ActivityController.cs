using Application.Activities;
using Application.Common.Models;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ActivitiesController : BaseController {

  /// <summary>
  /// Returns the list of all  Activities
  /// </summary>
  /// <param name="limit">Page size</param>
  /// <param name="offset">Starting index</param>
  /// <param name="isGoing">Activities you're participating</param>
  /// <param name="isHost">Activities your're hosting</param>
  /// <param name="startDate">Activity's starting date</param>
  /// <param name="ct">Cancellation Token</param>
  /// <returns></returns>
  /// <response code="200">Returns the requested books</response>
  [ProducesResponseType(StatusCodes.Status200OK)]
  [HttpGet]
  public async Task<ActionResult<PaginatedList<ActivityDto>>> GetAll(
    int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate, CancellationToken ct) =>
    await Mediator.Send(new GetAll.Query(limit ?? 3, offset ?? 0, isGoing, isHost, startDate ?? DateTime.Now), ct);

  /// <summary>
  /// Returns an activity
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
  [HttpGet("{activityId}")]
  public async Task<ActionResult<ActivityDetailDto>> Get(Guid activityId, CancellationToken ct) =>
    await Mediator.Send(new Details.Query(activityId), ct);

  /// <summary>
  /// Creates a new Activity
  /// </summary>
  /// <param name="command">A new activity</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<Guid>> Create([FromBody] Create.Command command, CancellationToken ct) {
    var model = await Mediator.Send(command, ct);
    return Created(nameof(ActivitiesController.Get), new { Id = model.Id });
  }

  /// <summary>
  /// Edit an activity
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="command">Activity</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPut("{activityId}")]
  [Authorize(Policy = IsHostRequirement.PolicyName)]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  public async Task<ActionResult> Edit(Guid activityId, Edit.Command command, CancellationToken ct) {
    command.Id = activityId;
    await Mediator.Send(command, ct);
    return Ok();
  }

  /// <summary>
  /// Partially edit an activity 
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="command">Activity</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPatch("{activityId}")]
  [Authorize(Policy = IsHostRequirement.PolicyName)]
  [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
  public async Task<ActionResult> EditPartial(Guid activityId, EditPartial.Command command, CancellationToken ct) {
    command.Id = activityId;
    await Mediator.Send(command, ct);
    return Ok();
  }

  /// <summary>
  /// Delete an activity
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpDelete("{activityId}")]
  [Authorize(Policy = IsHostRequirement.PolicyName)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<ActionResult> Delete(Guid activityId, CancellationToken ct) {
    await Mediator.Send(new Delete.Command(activityId), ct);
    return NoContent();
  }

  /// <summary>
  /// Attend an activity
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPost("{activityId}/attend")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<ActionResult> Attend(Guid activityId, CancellationToken ct) {
    await Mediator.Send(new Attend.Command(activityId), ct);
    return StatusCode(StatusCodes.Status201Created);
  }

  /// <summary>
  /// Unattend an activity
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpDelete("{activityId}/attend")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<ActionResult> Unattend(Guid activityId, CancellationToken ct) {
    await Mediator.Send(new Unattend.Command(activityId), ct);
    return NoContent();
  }
}