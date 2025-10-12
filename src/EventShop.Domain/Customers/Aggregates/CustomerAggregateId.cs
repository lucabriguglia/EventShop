using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Customers.Aggregates;

public record CustomerAggregateId(Guid CustomerId) : IAggregateId<Customer>
{
    public string Id => $"customer:{CustomerId}";
}
