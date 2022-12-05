using Application.Comments;
using Infrastructure.Security;
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
    command = command with { UserId = Context.User.GetUserId() };

    var comment = await _mediator.Send(command);

    await Clients.All.SendAsync("ReceiveComment", comment);
  }
}