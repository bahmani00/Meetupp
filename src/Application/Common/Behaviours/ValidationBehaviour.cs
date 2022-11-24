using FluentValidation;
using MediatR;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse> {
  private readonly IEnumerable<IValidator<TRequest>> _validators;

  public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) {
    _validators = validators;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) {
    if (!_validators.Any()) {
      return await next();
    }
    var context = new ValidationContext<TRequest>(request);

    var validationResults = await Task.WhenAll(
        _validators.Select(v =>
            v.ValidateAsync(context, ct)));

    var failures = validationResults
      .Where(r => r.Errors.Any())
      .Where(x => x != null)
      .SelectMany(x => x.Errors)
      .GroupBy(
        x => x.PropertyName,
        x => x.ErrorMessage,
        (propertyName, errorMessages) => new {
          Key = propertyName,
          Values = errorMessages.Distinct().ToArray()
        })
      .ToDictionary(x => x.Key, x => x.Values);

    if (failures.Any()) {
      throw new ValidationException(failures);
    }

    return await next();
  }
}