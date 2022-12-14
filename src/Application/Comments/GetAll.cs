using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments;

public static class GetAll {

  internal class Handler : IRequestHandler<Query, PaginatedList<CommentDto>> {
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    public Handler(IAppDbContext dbContext, IMapper mapper) {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    public async Task<PaginatedList<CommentDto>> Handle(Query request, CancellationToken ct) {
      return await dbContext.Comments
        .AsNoTracking()
        .Where(x => x.ActivityId == request.ActivityId)
        .Include(x => x.CreatedBy).ThenInclude(x => x.Photos)
        .OrderBy(x => x.CreatedOn)
        .AsQueryable()
        .TagWithCallSite()
        .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
        .PaginatedListAsync(request.Offset, request.Limit);
    }
  }

  public sealed record Query(
    Guid ActivityId,
    int Limit,
    int Offset) : IRequest<PaginatedList<CommentDto>>;
}