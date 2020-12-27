using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<List<ActivityDto>> 
        { 
        }

        public class Handler : IRequestHandler<Query, List<ActivityDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<List> _logger;
			private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<List> logger)
            {
                _mapper = mapper;
                _context = context;
                _logger = logger;
            }

            public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // try
                // {
                //     for(var i = 0; i < 5; i++){
                //         cancellationToken.ThrowIfCancellationRequested();
                //         await Task.Delay(1000, cancellationToken);
                //         _logger.LogInformation($"Task {i} has completed");
                //     }
                // }
                // catch (Exception ex) when(ex is TaskCanceledException)
                // {
                //     _logger.LogInformation("Task was cancelled.");
                // }

                var activities = await _context.Activities
                        .Include(x => x.UserActivities)
                        .ThenInclude(x => x.AppUser)
                        .ToListAsync(cancellationToken);

                return _mapper.Map<List<Activity>, List<ActivityDto>>(activities);
            }
        }
    }
}