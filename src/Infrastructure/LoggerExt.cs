using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class Log {

  public static void Info(this ILogger logger, string message) =>
    info(logger, message, null);

  public static void Debug(this ILogger logger, string message, Exception exc) =>
    debug(logger, message, exc);

  public static void Error(this ILogger logger, string message, Exception exc) =>
    error(logger, message, exc);

  private static readonly Action<ILogger, string, Exception?> info =
    LoggerMessage.Define<string>(
      LogLevel.Information,
      Events.Started,
      "{Param1}");

  private static readonly Action<ILogger, string, Exception> debug =
    LoggerMessage.Define<string>(
      LogLevel.Debug,
      Events.Started,
      "{Param1}");

  private static readonly Action<ILogger, string, Exception> error =
     LoggerMessage.Define<string>(
       LogLevel.Error,
       Events.Started,
       "{Param1}");
  internal static class Events {
    public static readonly EventId Started = new(100, "Started");
  }
}