using Application.Comments;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/Activities")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class CommentController : BaseController {
  /// <summary>
  /// Returns the list of all  Activities
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="limit">Page size</param>
  /// <param name="offset">Starting index</param>
  /// <param name="ct">Cancellation Token</param>
  /// <returns></returns>
  /// <response code="200">Returns the requested comments</response>
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [HttpGet("{activityId}/comments")]
  public async Task<ActionResult<PaginatedList<CommentDto>>> GetAll(Guid activityId,
    int? limit, int? offset, CancellationToken ct) =>
    Ok(await Mediator.Send(new GetAll.Query(activityId, limit ?? 3, offset ?? 0), ct));

  /// <summary>
  /// Returns a Comment
  /// </summary>
  /// <param name="commentId">Comment Id</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
  [HttpGet("{activityId}/comments/{commentId}")]
  public async Task<ActionResult<CommentDto>> Get(Guid commentId, CancellationToken ct) =>
    Ok(await Mediator.Send(new Get.Query(commentId), ct));

  /// <summary>
  /// Comment on an activity
  /// </summary>
  /// <param name="activityId">Activity Id</param>
  /// <param name="body">Body of comment</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPost("{activityId}/comment")]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
  public async Task<ActionResult<CommentDto>> Comment(Guid activityId, [FromBody] string body, CancellationToken ct) {
    var res = await Mediator.Send(new Create.Command(activityId, UserId!, body), ct);
    return StatusCode(StatusCodes.Status201Created, res);
  }
}