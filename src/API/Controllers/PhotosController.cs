using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PhotosController : BaseController {
  //by default it reads from FormBody, correct one FromForm
  [HttpPost]
  public async Task<ActionResult<PhotoDto>> Add([FromForm] Add.Command command, CancellationToken ct) =>
    Ok(await Mediator.Send(command, ct));

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(string id, CancellationToken ct) {
    await Mediator.Send(new Delete.Command { Id = id }, ct);
    return NoContent();
  }

  [HttpPost("{id}/setmain")]
  public async Task<ActionResult> SetMain(string id, CancellationToken ct) =>
    Ok(await Mediator.Send(new SetMain.Command { Id = id }, ct));
}