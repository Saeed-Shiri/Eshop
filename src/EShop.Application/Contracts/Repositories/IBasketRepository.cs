using Eshop.Domain.Entities;

namespace EShop.Application.Contracts.Repositories;
public interface IBasketRepository
{
    Task<Basket?> GetBasketAsync(Guid userId);
    Task UpdateBasketAsync(Basket basket);
    Task DeleteBasketAsync(Guid userId);
}
