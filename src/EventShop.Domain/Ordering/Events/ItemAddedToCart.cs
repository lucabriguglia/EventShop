using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Events;

[EventType("ItemAddedToCart")]
public record ItemAddedToCart(Guid ProductId, int Quantity, decimal UnitPrice) : IEvent;
