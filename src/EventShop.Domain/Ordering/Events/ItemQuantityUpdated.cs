using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Events;

[EventType("ItemQuantityUpdated")]
public record ItemQuantityUpdated(Guid ProductId, int NewQuantity) : IEvent;
