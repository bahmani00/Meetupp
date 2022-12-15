using FluentValidation;

namespace Application.Activities;

internal class ActivityValidator : AbstractValidator<ActivityBaseDto> {
  public ActivityValidator() {
    RuleFor(x => x.Title).NotEmpty();
    RuleFor(x => x.Description).NotEmpty();
    RuleFor(x => x.Date).NotEmpty();
    RuleFor(x => x.Category).NotEmpty();
    RuleFor(x => x.City).NotEmpty();
    RuleFor(x => x.Venue).NotEmpty();
  }
}