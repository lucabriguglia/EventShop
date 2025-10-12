using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Inventory.Events;

[EventType("ProductStockAdjusted")]
public record ProductStockAdjusted(Guid ProductId, int Quantity, int StockAfterAdjustment);
