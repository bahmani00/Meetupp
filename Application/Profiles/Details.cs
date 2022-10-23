using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Profiles;
public class Details {
    public class Query : IRequest<Profile> {
        public string Username { get; set; }
    }

    public class Handler : IRequestHandler<Query, Profile> {
        private readonly IProfileReader _profileReader;
        public Handler(IProfileReader profileReader) {
            _profileReader = profileReader;
        }

        public async Task<Profile> Handle(Query request, CancellationToken ct) {
            return await _profileReader.ReadProfile(request.Username);
        }
    }
}