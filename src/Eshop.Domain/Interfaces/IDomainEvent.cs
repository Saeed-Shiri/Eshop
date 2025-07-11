
namespace Eshop.Domain.Interfaces;
public interface IDomainEvent
{
    public DateTime OccurredOn { get; }
}
