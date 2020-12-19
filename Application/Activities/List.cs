using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Microsoft.Extensions.Logging;
using System;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<List<Activity>> 
        { 
        }

        public class Handler : IRequestHandler<Query, List<Activity>>
        {
            private readonly FacebukDbContext _context;
            private readonly ILogger<List> _logger;
            public Handler(FacebukDbContext context, ILogger<List> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    for(var i = 0; i < 5; i++){
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(1000, cancellationToken);
                        _logger.LogInformation($"Task {i} has completed");
                    }
                }
                catch (Exception ex) when(ex is TaskCanceledException)
                {
                    _logger.LogInformation("Task was cancelled.");
                }

                var activities = await _context.Activities.ToListAsync(cancellationToken);

                return activities;
            }
        }
    }
}