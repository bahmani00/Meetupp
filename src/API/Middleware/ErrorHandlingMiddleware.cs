using Application.Common.Exceptions;
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
    object? errors;
    switch (ex) {
      case RestException re:
        _logger.Error("REST ERROR", ex);
        errors = re.Errors;
        context.Response.StatusCode = (int)re.Code;
        break;
      default:
        _logger.Error("SERVER ERROR", ex);
        errors = string.IsNullOrWhiteSpace(ex.Message) ? "Error" : ex.Message;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
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