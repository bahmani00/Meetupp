using System;
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
      int? offset, bool isGoing, bool isHost, DateTime? startDate) {
    return await Mediator.Send(new List.Query(limit,
        offset, isGoing, isHost, startDate));
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<ActivityDto>> Details(Guid id) {
    return await Mediator.Send(new Details.Query { Id = id });
  }

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(Create.Command command) {
    return await Mediator.Send(command);
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "IsHostCreatedActivity")]
  public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command) {
    command.Id = id;
    return await Mediator.Send(command);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Unit>> EditPartial(Guid id, EditPartial.Command cmd) {
    cmd.Id = id;
    return await Mediator.Send(cmd);
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "IsHostCreatedActivity")]
  public async Task<ActionResult<Unit>> Delete(Guid id) {
    return await Mediator.Send(new Delete.Command { Id = id });
  }

  [HttpPost("{id}/attend")]
  public async Task<ActionResult<Unit>> Attend(Guid id) {
    return await Mediator.Send(new Attend.Command { Id = id });
  }

  [HttpDelete("{id}/attend")]
  public async Task<ActionResult<Unit>> Unattend(Guid id) {
    return await Mediator.Send(new Unattend.Command { Id = id });
  }
}
