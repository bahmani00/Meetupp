using Application.Photos;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PhotosController : BaseController {
  //by default it reads from FormBody, correct one FromForm
  [HttpPost]
  public async Task<ActionResult<Photo>> Add([FromForm] Add.Command command, CancellationToken ct) {
    return await Mediator.Send(command, ct);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Unit>> Delete(string id, CancellationToken ct) {
    return await Mediator.Send(new Delete.Command { Id = id }, ct);
  }

  [HttpPost("{id}/setmain")]
  public async Task<ActionResult<Unit>> SetMain(string id, CancellationToken ct) {
    return await Mediator.Send(new SetMain.Command { Id = id }, ct);
  }
}