using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase {
  private readonly DataContext dbContext;
  private readonly ILogger<WeatherForecastController> logger;

  public WeatherForecastController(DataContext dbContext, ILogger<WeatherForecastController> logger) {
    this.dbContext = dbContext;
    this.logger = logger;
  }

  [HttpGet]
  public async Task<IActionResult> Get(CancellationToken ct) {
    var list = await dbContext.WeatherForecasts.ToListAsync(ct);
    logger.LogInformation("WeatherForecastController.GetAll is called.");

    return Ok(list);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> Get(int id, CancellationToken ct) {
    var entity = await dbContext.WeatherForecasts.FindItemAsync(id, ct);
    logger.LogInformation($"WeatherForecastController.GetById={id} is called.");
    //_loggerMessage(this._logger, "Person cannot be null.",  null);
    if (entity == null)
      return new StatusCodeResult(202);

    return Ok(entity);
  }

  //private static Action<ILogger, string, Exception> _loggerMessage = 
  //  LoggerMessage.Define<string>(LogLevel.Error, eventId: new EventId(id: 0, name: "ERROR"), formatString: "{Message}");
}