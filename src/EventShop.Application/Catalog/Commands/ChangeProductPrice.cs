using EventShop.Domain.Catalog.Aggregates;
using EventShop.Domain.Streams;
using FluentValidation;
using Memoria.Commands;
using Memoria.EventSourcing;
using Memoria.Results;

namespace EventShop.Application.Catalog.Commands;

public record ChangeProductPrice(Guid ProductId, decimal NewPrice) : ICommand;

public class ChangeProductPriceValidator : AbstractValidator<ChangeProductPrice>
{
    public ChangeProductPriceValidator()
    {
        RuleFor(c => c.NewPrice).GreaterThanOrEqualTo(0).WithMessage("New price can't be negative.");
    }
}

public class ChangeProductPriceHandler(IDomainService domainService) : ICommandHandler<ChangeProductPrice>
{
    public async Task<Result> Handle(ChangeProductPrice command, CancellationToken cancellationToken = default)
    {
        var streamId = new ProductStreamId(command.ProductId);
        var aggregateId = new ProductAggregateId(command.ProductId);

        var latestEventSequence = await domainService.GetLatestEventSequence(streamId, cancellationToken: cancellationToken);
        if (latestEventSequence.IsNotSuccess)
        {
            return latestEventSequence.Failure!;
        }

        var productResult = await ValidateAndRetrieveProduct(command.ProductId, cancellationToken);
        if (productResult.IsNotSuccess)
        {
            return productResult.Failure!;
        }
        var product = productResult.Value!;

        var changePriceResult = product.ChangePrice(command.NewPrice);
        if (changePriceResult.IsNotSuccess)
        {
            return changePriceResult.Failure!;
        }

        return await domainService.SaveAggregate(streamId, aggregateId, product, expectedEventSequence: latestEventSequence.Value, cancellationToken: cancellationToken);
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
