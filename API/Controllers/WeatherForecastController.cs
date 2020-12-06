using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly FacebukDbContext dbContext;
        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(FacebukDbContext dbContext, ILogger<WeatherForecastController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await dbContext.WeatherForecasts.ToListAsync();
            logger.LogInformation("WeatherForecastController.GetAll is called.");

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await dbContext.WeatherForecasts.FindAsync(id);
            logger.LogInformation($"WeatherForecastController.GetById={id} is called.");
            if (entity == null)
                return new StatusCodeResult(202);

            return Ok(entity);
        }
    }
}
