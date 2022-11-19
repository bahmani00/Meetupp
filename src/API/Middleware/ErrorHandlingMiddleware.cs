using System.Net;
using Application.Errors;
using Infrastructure;
using Newtonsoft.Json;

namespace API.Middleware;

public class ErrorHandlingMiddleware {
  private readonly RequestDelegate _next;
  private readonly ILogger _logger;

  public ErrorHandlingMiddleware(
      RequestDelegate next,
      ILogger<ErrorHandlingMiddleware> logger) {
    _logger = logger;
    _next = next;
  }

  public async Task Invoke(HttpContext context) {
    try {
      await _next(context);
    } catch (Exception ex) {
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception ex) {
    object errors = null;

    switch (ex) {
      case RestException re:
        _logger.Error("REST ERROR", ex);
        errors = re.Errors;
        context.Response.StatusCode = (int)re.Code;
        break;
      case Exception e:
        _logger.Error("SERVER ERROR", ex);
        errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        break;
    }

    context.Response.ContentType = "application/json";
    if (errors != null) {
      var result = JsonConvert.SerializeObject(new {
        errors
      });

      await context.Response.WriteAsync(result);
    }
  }
}