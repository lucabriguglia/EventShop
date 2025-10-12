using EventShop.Domain.Inventory.Events;
using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Inventory.Aggregates;

[AggregateType("InventoryItem")]
public class InventoryItem : AggregateRoot
{
    public override Type[] EventTypeFilter =>
    [
        typeof(ProductStockAdjusted)
    ];

    protected override bool Apply<T>(T @event)
    {
        throw new NotImplementedException();
    }
}
