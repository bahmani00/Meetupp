using Application.Common.Security;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BaseController : ControllerBase {
  private IMediator mediator;
  protected IMediator Mediator =>
    mediator ??= HttpContext.RequestServices.GetService<IMediator>();

  protected string UserName => User.Identity?.Name;
  protected string UserId => this.HttpContext.User.GetUserId();
}