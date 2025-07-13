
using Eshop.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Domain.Common;
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }


    protected BaseEntity()
    {
        
    }

    protected BaseEntity(Guid id)
    {
        Id = id == Guid.Empty ? throw new DomainException("Id cannot be empty") : id;
    }
}