using FluentValidation;

namespace Application.Auth;

public static class ValidatorExtensions {
  public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder) {
    var options = ruleBuilder
        .NotEmpty()
        .MinimumLength(6)
        .Matches("[A-Z]").WithMessage("Password must contain uppercase letter")
        .Matches("[a-z]").WithMessage("Password must contain lowercase letter")
        .Matches("[0-9]").WithMessage("Password must contain number")
        .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain non alphanumeric");

    return options;
  }
}