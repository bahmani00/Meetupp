using System.Net;

namespace Application.Errors;

public class RestException : Exception {
  public RestException(HttpStatusCode code, object errors = null) {
    Code = code;
    Errors = errors;
  }

  public HttpStatusCode Code { get; }
  public object Errors { get; }

  public static void ThrowIfBadRequest(bool @throw, object errors) {
    if (@throw)
      throw new RestException(HttpStatusCode.BadRequest, errors);
  }

  public static void ThrowIfNotFound<T>(T obj, object errors) {
    if (obj is null)
      throw new RestException(HttpStatusCode.NotFound, errors);
  }
}