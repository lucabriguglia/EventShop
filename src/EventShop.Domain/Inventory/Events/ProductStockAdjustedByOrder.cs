using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Inventory.Events;

[EventType("ProductStockAdjustedByOrder")]
public record ProductStockAdjustedByOrder(Guid OrderId, Guid ProductId, int Quantity, int StockAfterAdjustment);
