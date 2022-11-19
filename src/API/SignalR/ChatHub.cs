using System.Security.Claims;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class ChatHub : Hub {
  private readonly IMediator _mediator;
  public ChatHub(IMediator mediator) {
    _mediator = mediator;
  }

  public async Task SendComment(Create.Command command) {
    //auth token must have been added already to HubContext
    // how do u garantee that?
    var username = Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

    command.Username = username;

    var comment = await _mediator.Send(command);

    await Clients.All.SendAsync("ReceiveComment", comment);
  }
}