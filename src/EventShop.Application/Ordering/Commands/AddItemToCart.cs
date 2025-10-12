using EventShop.Domain.Catalog.Aggregates;
using EventShop.Domain.Ordering.Aggregates;
using EventShop.Domain.Streams;
using FluentValidation;
using Memoria.Commands;
using Memoria.EventSourcing;
using Memoria.Results;

namespace EventShop.Application.Ordering.Commands;

public record AddItemToCart(Guid CustomerId, Guid ShoppingCartId, Guid ProductId, int Quantity) : ICommand;

public class AddItemToCartValidator : AbstractValidator<AddItemToCart>
{
    public AddItemToCartValidator()
    {
        RuleFor(c => c.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

public class AddItemToCartHandler(IDomainService domainService) : ICommandHandler<AddItemToCart>
{
    public async Task<Result> Handle(AddItemToCart command, CancellationToken cancellationToken = default)
    {
        var productResult = await ValidateAndRetrieveProduct(command.ProductId, cancellationToken);
        if (productResult.IsNotSuccess)
        {
            return productResult.Failure!;
        }

        var streamId = new CustomerStreamId(command.CustomerId);
        var aggregateId = new ShoppingCartAggregateId(command.ShoppingCartId);

        var latestEventSequence = await domainService.GetLatestEventSequence(streamId, cancellationToken: cancellationToken);
        if (latestEventSequence.IsNotSuccess)
        {
            return latestEventSequence.Failure!;
        }

        var shoppingCartResult = await domainService.GetAggregate(streamId, aggregateId, cancellationToken: cancellationToken);
        if (shoppingCartResult.IsNotSuccess)
        {
            return shoppingCartResult.Failure!;
        }

        var shoppingCart = shoppingCartResult.Value is null
            ? new ShoppingCart(command.ShoppingCartId)
            : shoppingCartResult.Value!;

        shoppingCart.AddItem(command.ProductId, command.Quantity, productResult.Value!.Price);

        return await domainService.SaveAggregate(streamId, aggregateId, shoppingCart, expectedEventSequence: latestEventSequence.Value, cancellationToken: cancellationToken);
    }

    private async Task<Result<Product>> ValidateAndRetrieveProduct(Guid productId, CancellationToken cancellationToken)
    {
        var productStreamId = new ProductStreamId(productId);
        var productAggregateId = new ProductAggregateId(productId);

        var productResult = await domainService.GetAggregate(productStreamId, productAggregateId, cancellationToken: cancellationToken);
        if (productResult.IsNotSuccess)
        {
            return productResult.Failure!;
        }
        if (productResult.Value == null)
        {
            return new Failure(ErrorCode.NotFound, Title: "Product not found", Description: $"Product with ID {productId} not found.");
        }

        return productResult.Value;
    }
}
