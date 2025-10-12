using EventShop.Domain.Catalog.Events;
using Memoria.EventSourcing.Domain;
using Memoria.Results;

namespace EventShop.Domain.Catalog.Aggregates;

[AggregateType("Product")]
public class Product : AggregateRoot
{
    public override Type[] EventTypeFilter =>
    [
        typeof(ProductCreated),
        typeof(ProductPriceChanged)
    ];

    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public Product()
    {
    }

    public Product(Guid productId, string name, string description, decimal price)
    {
        Add(new ProductCreated(productId, name, description, price));
    }

    public Result ChangePrice(decimal newPrice)
    {
        if (newPrice == Price)
        {
            return new Failure(Title: "Price not changed", Description: "The new price is the same as the current price.");
        }

        Add(new ProductPriceChanged(ProductId, newPrice));

        return Result.Ok();
    }

    protected override bool Apply<T>(T domainEvent)
    {
        return domainEvent switch
        {
            ProductCreated @event => Apply(@event),
            ProductPriceChanged @event => Apply(@event),
            _ => false
        };
    }

    private bool Apply(ProductCreated @event)
    {
        ProductId = @event.ProductId;
        Name = @event.Name;
        Description = @event.Description;
        Price = @event.Price;

        return true;
    }

    private bool Apply(ProductPriceChanged @event)
    {
        Price = @event.NewPrice;

        return true;
    }
}
