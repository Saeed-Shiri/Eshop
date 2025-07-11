
namespace Eshop.Domain.Exceptions;
public class ProductNotAvailabeException(Guid id) : DomainException($"Product with Id '{id}' is not available");

public class ProductNotLockedException(Guid id) : DomainException($"Product with Id '{id}' is not locked");

public class ProductReleaseLockException(Guid id) : DomainException($"Product with Id '{id}' firstly must be locked before release lock");

public class ProducNameNullException() : DomainException("Name cannot be null");

public class ProductPriceNegativeException() : DomainException("Price cannot be positive");
