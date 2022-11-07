namespace Application.Auth;

public interface IUserAccessor {
  string GetCurrentUsername();
}