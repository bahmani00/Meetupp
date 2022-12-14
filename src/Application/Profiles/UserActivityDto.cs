namespace Application.Profiles;

public class UserActivityDto {
  public Guid Id { get; set; }

  public string Title { get; set; } = null!;
  public string Category { get; set; } = null!;

  public DateTime Date { get; set; }
}