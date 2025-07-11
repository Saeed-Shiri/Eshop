

using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Common;
public class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid id) : base(id) { }

    protected void Publish<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        _domainEvents.Add(@event);
    }

    public IReadOnlyCollection<IDomainEvent> GetChanges() => _domainEvents.AsReadOnly();

    public void ClearChanges() => _domainEvents.Clear();

}