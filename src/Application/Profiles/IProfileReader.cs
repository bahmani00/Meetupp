using System.Threading;
using System.Threading.Tasks;

namespace Application.Profiles;

public interface IProfileReader {
  Task<Profile> ReadProfileAsync(string username, CancellationToken ct);
}