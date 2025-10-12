using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Aggregates;

public record ShoppingCartAggregateId(Guid ShoppingCartId) : IAggregateId<ShoppingCart>
{
    public string Id => $"shopping-cart:{ShoppingCartId}";
}
