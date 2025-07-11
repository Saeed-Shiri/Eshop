

namespace Eshop.Domain.Interfaces.Services;
public interface IProductLockService
{
    /// <summary>
    /// قفل کردن محصول برای مدت مشخص (مثلاً ۱۰ دقیقه)
    /// </summary>
    Task LockProductAsync(Guid productId, TimeSpan duration);

    /// <summary>
    /// آزادسازی قفل محصول (در صورت پرداخت ناموفق یا انقضا)
    /// </summary>
    Task UnlockProductAsync(Guid productId);
}
