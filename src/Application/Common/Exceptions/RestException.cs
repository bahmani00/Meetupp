using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class RestException : Exception {
  public RestException(int code, object? errors = null) {
    Code = code;
    Errors = errors;
  }

  public int Code { get; }
  public object? Errors { get; }

  public static void ThrowIfBadRequest(bool @throw, object errors) {
    if (@throw)
      throw new RestException(StatusCodes.Status400BadRequest, errors);
  }

  public static void ThrowIfNotFound<T>(T obj, object errors) {
    if (obj is null)
      throw new RestException(StatusCodes.Status404NotFound, errors);
  }
}