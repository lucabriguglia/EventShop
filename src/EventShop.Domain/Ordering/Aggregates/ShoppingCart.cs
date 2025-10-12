using EventShop.Domain.Ordering.Events;
using Newtonsoft.Json;
using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Ordering.Aggregates;

[AggregateType("ShoppingCart")]
public class ShoppingCart : AggregateRoot
{
    public override Type[] EventTypeFilter =>
    [
        typeof(ItemAddedToCart)
    ];

    public Guid ShoppingCartId { get; private set; }

    [JsonProperty(nameof(ShoppingCartItems))]
    private readonly List<ShoppingCartItem> _shoppingCartItems = [];
    [JsonIgnore]
    public IEnumerable<ShoppingCartItem> ShoppingCartItems => _shoppingCartItems.AsReadOnly();

    public ShoppingCart()
    {
    }

    public ShoppingCart(Guid shoppingCartId)
    {
        Add(new ShoppingCartCreated(shoppingCartId));
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var existingItem = _shoppingCartItems.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            Add(new ItemQuantityUpdated(productId, newQuantity));
        }
        else
        {
            Add(new ItemAddedToCart(productId, quantity, unitPrice));
        }
    }

    protected override bool Apply<T>(T domainEvent)
    {
        return domainEvent switch
        {
            ShoppingCartCreated @event => Apply(@event),
            ItemAddedToCart @event => Apply(@event),
            ItemQuantityUpdated @event => Apply(@event),
            _ => false
        };
    }

    private bool Apply(ShoppingCartCreated @event)
    {
        ShoppingCartId = @event.ShoppingCartId;

        return true;
    }

    private bool Apply(ItemAddedToCart @event)
    {
        _shoppingCartItems.Add(new ShoppingCartItem
        {
            ProductId = @event.ProductId,
            Quantity = @event.Quantity,
            UnitPrice = @event.UnitPrice
        });

        return true;
    }

    private bool Apply(ItemQuantityUpdated @event)
    {
        var existingItem = _shoppingCartItems.FirstOrDefault(i => i.ProductId == @event.ProductId);
        if (existingItem == null)
        {
            return false;
        }

        existingItem.Quantity = @event.NewQuantity;

        return true;
    }
}

public class ShoppingCartItem
{
    public required Guid ProductId { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
}
