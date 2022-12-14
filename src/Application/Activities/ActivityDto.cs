using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Comments;

namespace Application.Activities;

public class ActivityBaseDto {
  /// <summary>
  /// Activity Title
  /// </summary>
  public virtual string Title { get; set; } = null!;

  /// <summary>
  /// Activity Description
  /// </summary>
  public virtual string Description { get; set; } = null!;

  /// <summary>
  /// Activity Category
  /// </summary>
  public virtual string Category { get; set; } = null!;

  /// <summary>
  /// Activity Date
  /// </summary>
  public virtual DateTime Date { get; set; }

  /// <summary>
  /// Activity City
  /// </summary>
  public virtual string City { get; set; } = null!;

  /// <summary>
  /// Activity Venue
  /// </summary>
  public virtual string Venue { get; set; } = null!;

  public bool DateHasValue() => Date > DateTime.Now;
}

public class ActivityBaseRequiredDto: ActivityBaseDto {
  /// <inheritdoc />
  [Required]
  public required override string Title { get; set; }

  /// <inheritdoc />
  [Required]
  public required override string Description { get; set; }

  /// <inheritdoc />
  [Required]
  public required override string Category { get; set; }

  /// <inheritdoc />
  [Required]
  public required override DateTime Date { get; set; }

  /// <inheritdoc />
  [Required]
  public required override string City { get; set; }

  /// <inheritdoc />
  [Required]
  public required override string Venue { get; set; }
}

/// <summary>
/// Activity with attendees
/// </summary>
public class ActivityDto : ActivityBaseDto {
  /// <summary>
  /// Activity Id
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Activity attendees
  /// </summary>
  [JsonPropertyName("attendees")]
  public IEnumerable<AttendeeDto> UserActivities { get; set; } = null!;
}

/// <summary>
/// Activity with attendees and comments
/// </summary>
public class ActivityDetailDto : ActivityDto {
  /// <summary>
  /// All the comments on this activity
  /// </summary>
  public ICollection<CommentDto> Comments { get; set; } = null!;
}