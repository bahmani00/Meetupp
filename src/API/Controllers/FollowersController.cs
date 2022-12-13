using Application.Followers;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/profiles")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class FollowersController : BaseController {

  /// <summary>
  /// Follow requested user
  /// </summary>
  /// <param name="userId">User Id to be followed</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpPost("{userId}/follow")]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
  public async Task<ActionResult> Follow(string userId, CancellationToken ct) {

    await Mediator.Send(new Add.Command(userId), ct);
    //var createdResource = new { id = userId };
    //var routeValues = new {
    //  action = nameof(FollowersController.GetFollowings),
    //  controller = "Followers",
    //  //id = createdResource.Id,
    //  //predicate = createdResource.predicate,
    //};
    //return CreatedAtAction(nameof(GetFollowings), createdResource, "following");
    //return Created(nameof(GetFollowings), );
    return StatusCode(StatusCodes.Status201Created);
  }


  /// <summary>
  /// Unfollow requested user
  /// </summary>
  /// <param name="userId">User Id to be unfollowed</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpDelete("{userId}/follow")]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
  public async Task<ActionResult> Unfollow(string userId, CancellationToken ct) {
    await Mediator.Send(new Delete.Command(userId), ct);
    return NoContent();
  }

  /// <summary>
  /// Get profile of Followers/Followings requested user
  /// </summary>
  /// <param name="userId">User Id of profile user</param>
  /// <param name="predicate">Either fllower or following</param>
  /// <param name="ct"></param>
  /// <returns></returns>
  [HttpGet("{userId}/follow")]
  [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(List<Profile>), StatusCodes.Status200OK)]
  public async Task<ActionResult<List<Profile>>> GetFollowings(string userId, [FromQuery] string predicate, CancellationToken ct) =>
    Ok(await Mediator.Send(new List.Query(userId, predicate), ct));
}