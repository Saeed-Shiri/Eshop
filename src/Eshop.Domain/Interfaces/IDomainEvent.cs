
namespace Eshop.Domain.Interfaces;
public interface IDomainEvent
{
    public DateTime OccurredOn { get; }
}

public abstract record DomainEventBase :IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
