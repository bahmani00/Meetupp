using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase {
  private IMediator mediator;
  protected IMediator Mediator =>
    mediator ??= HttpContext.RequestServices.GetService<IMediator>();
}