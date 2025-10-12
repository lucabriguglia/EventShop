using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Events;

[EventType("ShoppingCartCreated")]
public record ShoppingCartCreated(Guid ShoppingCartId) : IEvent;
