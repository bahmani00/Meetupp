using System.Net;

namespace Application.Errors;

public class RestException : Exception {
  public RestException(HttpStatusCode code, object errors = null) {
    Code = code;
    Errors = errors;
  }

  public HttpStatusCode Code { get; }
  public object Errors { get; }

  public static void ThrowBadRequest(object errors) =>
    throw new RestException(HttpStatusCode.BadRequest, errors);

  public static void ThrowNotFound(object errors) =>
    throw new RestException(HttpStatusCode.NotFound, errors);
}