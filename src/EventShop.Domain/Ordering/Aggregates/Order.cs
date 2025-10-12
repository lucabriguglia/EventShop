using EventShop.Domain.Catalog.Events;
using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Aggregates;

[AggregateType("Order")]
public class Order : AggregateRoot
{
    public override Type[] EventTypeFilter =>
    [
        typeof(ProductCreated),
        typeof(ProductPriceChanged)
    ];

    protected override bool Apply<T>(T @event)
    {
        throw new NotImplementedException();
    }
}
