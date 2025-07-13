

namespace EShop.Application.Contracts.Services;
public interface IProductLockService
{
    /// <summary>
    /// قفل کردن محصول برای مدت مشخص (مثلاً ۱۰ دقیقه)
    /// </summary>
    Task LockProductAsync(Guid productId, TimeSpan duration);

    /// <summary>
    /// آزادسازی قفل محصول (در صورت پرداخت موفق یا انقضا)
    /// </summary>
    Task UnlockProductAsync(Guid productId);

    /// <summary>
    /// برگرداندن مدت زمان قفل محصول
    /// </summary>
    Task<DateTime?> GetLockExpiryAsync(Guid productId);
}
