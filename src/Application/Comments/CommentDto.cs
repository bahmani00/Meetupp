namespace Application.Comments;

public class CommentDto {
  public Guid Id { get; set; }

  public string Body { get; set; } = null!;
  public DateTime CreatedOn { get; set; }

  //commenter's profile
  public string Username { get; set; } = null!;
  public string DisplayName { get; set; } = null!;
  //commenter's main image profile
  public string? Image { get; set; }
}