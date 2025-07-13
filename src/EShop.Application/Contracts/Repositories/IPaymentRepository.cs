

using Eshop.Domain.Entities;

namespace EShop.Application.Contracts.Repositories;
public interface IPaymentRepository
{
    Task<Payment> AddAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
}
