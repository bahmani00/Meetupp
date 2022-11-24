using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase {
  private IMediator mediator;
  protected IMediator Mediator =>
    mediator ??= HttpContext.RequestServices.GetService<IMediator>();

  protected string GetCurrUserName() => this.HttpContext.User.GetUsername();
}