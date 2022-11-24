using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception {
  public ValidationException()
      : base("One or more validation failures have occurred.") {
    Errors = new Dictionary<string, string[]>();
  }

  public ValidationException(IEnumerable<ValidationFailure> failures)
      : this() {
    Errors = failures
        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
        .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
  }

  public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
      //: base("Validation Failure", "One or more validation errors occurred")
      => Errors = errorsDictionary;

  public IReadOnlyDictionary<string, string[]> Errors { get; }
}