using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Comments;

namespace Application.Activities;

public class ActivityBaseDto {
  [Required]
  public string Title { get; set; }
  [Required]
  public string Description { get; set; }
  [Required]
  public string Category { get; set; }
  [Required]
  public DateTime? Date { get; set; }
  [Required]
  public string City { get; set; }
  [Required]
  public string Venue { get; set; }
}

public class ActivityDto : ActivityBaseDto {
  public Guid Id { get; set; }

  [JsonPropertyName("attendees")]
  public ICollection<AttendeeDto> UserActivities { get; set; }

  /// <summary>
  /// All the comments on this activity
  /// </summary>
  public ICollection<CommentDto> Comments { get; set; }
}