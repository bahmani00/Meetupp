using MediatR;

namespace Application.Profiles;

public static class Details {

  public class Handler : IRequestHandler<Query, Profile> {
    private readonly IProfileReader profileReader;

    public Handler(IProfileReader profileReader) {
      this.profileReader = profileReader;
    }

    public async Task<Profile> Handle(Query request, CancellationToken ct) {
      return await profileReader.ReadProfileAsync(request.Username, ct);
    }
  }

  public record Query(string Username) : IRequest<Profile>;
}