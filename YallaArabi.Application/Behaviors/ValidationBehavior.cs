using System.Reflection;
using FluentValidation;
using Mediator;

namespace YallaArabi.Application.Behaviors;

public sealed class ValidationBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly IEnumerable<IValidator<TMessage>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var context = new ValidationContext<TMessage>(message);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(message, cancellationToken);
        }

        var errorMessage = string.Join(" | ", failures.Select(f => f.ErrorMessage));

        var failureMethod = typeof(TResponse).GetMethod("Failure", BindingFlags.Public | BindingFlags.Static);
        if (failureMethod is null)
        {
            throw new ValidationException(failures);
        }

        return (TResponse)failureMethod.Invoke(null, new object[] { "validation_error", errorMessage })!;
    }
}