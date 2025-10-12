using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Events;

[EventType("OrderPlaced")]
public record OrderPlaced(Guid OrderId, OrderItem[] OrderItems, decimal TotalCost);
public record OrderItem(Guid ProductId, int Quantity, decimal UnitPrice);
