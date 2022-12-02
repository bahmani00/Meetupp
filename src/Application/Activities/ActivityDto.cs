using System.Text.Json.Serialization;
using Application.Comments;
using Domain;

namespace Application.Activities;

public class ActivityBaseDto {
  public string Title { get; set; }
  public string Description { get; set; }
  public string Category { get; set; }
  public DateTime? Date { get; set; }
  public string City { get; set; }
  public string Venue { get; set; }

  public Activity ToEntity(Activity entity) {
    entity.Title = Title;
    entity.Description = Description;
    entity.Category = Category;
    entity.Date = Date.Value;
    entity.City = City;
    entity.Venue = Venue;
    return entity;
  }
  public Activity ToEntity() => ToEntity(new Activity());

  public Activity ToEntityPartial(Activity entity) {
    entity.Title = Title ?? entity.Title;
    entity.Description = Description ?? entity.Description;
    entity.Category = Category;
    entity.Date = Date ?? entity.Date;
    entity.City = City ?? entity.City;
    entity.Venue = Venue ?? entity.Venue;
    return entity;
  }
}

public class ActivityDto : ActivityBaseDto {
  public Guid Id { get; set; }

  [JsonPropertyName("attendees")]
  public ICollection<AttendeeDto> UserActivities { get; set; }

  public ICollection<CommentDto> Comments { get; set; }
}