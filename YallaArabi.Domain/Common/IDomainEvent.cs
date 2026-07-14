namespace YallaArabi.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}