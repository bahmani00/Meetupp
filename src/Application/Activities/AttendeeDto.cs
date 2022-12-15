namespace Application.Activities;

public class AttendeeDto {
  public string UserId { get; set; } = null!;
  public string Username { get; set; } = null!;
  public string DisplayName { get; set; } = null!;

  public string? Image { get; set; }

  public bool IsHost { get; set; }
  public bool Following { get; set; }
}