using Eshop.Domain.Entities;

namespace Eshop.Domain.Interfaces.Repositories;
public interface IBasketRepository
{
    Task<Basket> GetBasketAsync(Guid userId);
    Task UpdateBasketAsync(Basket basket);
    Task DeleteBasketAsync(Guid userId);
}
