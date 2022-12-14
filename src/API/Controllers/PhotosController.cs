using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class PhotosController : BaseController {
  /// <summary>
  /// Add Photo
  /// </summary>
  /// <param name="command"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  //by default it reads from FormBody, correct one FromForm
  [HttpPost]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(PhotoDto), StatusCodes.Status201Created)]
  public async Task<ActionResult<PhotoDto>> Create([FromForm] Add.Command command, CancellationToken ct) {
    var res = await Mediator.Send(command, ct);
    return StatusCode(StatusCodes.Status201Created, res);
  }

  /// <summary>
  /// Delete your photo by photoId
  /// </summary>
  /// <param name="photoId">A unique id for Photo</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpDelete("{photoId}")]
  [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<ActionResult> Delete(string photoId, CancellationToken ct) {
    await Mediator.Send(new Delete.Command { Id = photoId }, ct);
    return NoContent();
  }

  /// <summary>
  /// Set your profile photo
  /// </summary>
  /// <param name="photoId">Photo Id</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPost("{photoId}/setmain")]
  [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult> SetMain(string photoId, CancellationToken ct) {
    await Mediator.Send(new SetMain.Command { Id = photoId }, ct);
    return Ok();
  }
}