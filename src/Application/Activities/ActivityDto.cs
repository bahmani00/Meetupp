using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Comments;

namespace Application.Activities;

public class ActivityBaseDto {
  [Required]
  public string Title { get; set; } = null!;
  [Required]
  public string Description { get; set; } = null!;
  [Required]
  public string Category { get; set; } = null!;
  [Required]
  public DateTime? Date { get; set; } = null!;
  [Required]
  public string City { get; set; } = null!;
  [Required]
  public string Venue { get; set; } = null!;
}

public class ActivityDto : ActivityBaseDto {
  public Guid Id { get; set; }

  [JsonPropertyName("attendees")]
  public ICollection<AttendeeDto> UserActivities { get; set; } = null!;

  /// <summary>
  /// All the comments on this activity
  /// </summary>
  public ICollection<CommentDto> Comments { get; set; } = null!;
}