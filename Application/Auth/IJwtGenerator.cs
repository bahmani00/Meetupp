using Domain;

namespace Application.Auth;

public interface IJwtGenerator {
  string CreateToken(AppUser user);
}