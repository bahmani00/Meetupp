using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Exceptions.RestException;

namespace Application.Comments;

public static class Get {

  public class Handler : IRequestHandler<Query, CommentDto> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<CommentDto> Handle(Query request, CancellationToken ct) {
      var entity = await dbContext.Comments
        .Include(x => x.CreatedBy).ThenInclude(x => x.Photos)
        .SingleOrDefaultAsync(x => x.Id == request.Id, ct);

      ThrowIfNotFound(entity, new { Comment = "Not found" });

      return mapper.Map<CommentDto>(entity);
    }
  }

  public record Query(Guid Id) : IRequest<CommentDto>;
}