using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityContoller : ControllerBase
    {
        private readonly IMediator mediator;

        public ActivityContoller(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<List<Activity>> Get()
        {
            return await mediator.Send(new List.Query());
        }
    }
}
