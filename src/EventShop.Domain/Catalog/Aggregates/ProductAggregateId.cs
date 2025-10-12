using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Catalog.Aggregates;

public record ProductAggregateId(Guid ProductId) : IAggregateId<Product>
{
    public string Id => $"product:{ProductId}";
}
