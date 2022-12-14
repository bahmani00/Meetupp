namespace Application.Auth;

public class UserDto {
  public string Username { get; set; } = null!;
  public string DisplayName { get; set; } = null!;

  public string Token { get; set; } = null!;

  public string? Image { get; set; }
}