using EventShop.Domain.Catalog.Aggregates;
using EventShop.Domain.Streams;
using FluentValidation;
using Memoria.Commands;
using Memoria.EventSourcing;
using Memoria.Results;

namespace EventShop.Application.Catalog.Commands;

public record CreateProduct(string Name, string Description, decimal Price) : ICommand<Guid>;

public class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(c => c.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(c => c.Price).NotEmpty().WithMessage("Price is required.");
        RuleFor(c => c.Price).GreaterThanOrEqualTo(0).WithMessage("Price can't be negative.");
    }
}

public class CreateProductHandler(IDomainService domainService) : ICommandHandler<CreateProduct, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProduct command, CancellationToken cancellationToken = default)
    {
        var productId = Guid.NewGuid();
        var streamId = new ProductStreamId(productId);
        var aggregateId = new ProductAggregateId(productId);
        var aggregate = new Product(productId, command.Name, command.Description, command.Price);
        await domainService.SaveAggregate(streamId, aggregateId, aggregate, expectedEventSequence: 0, cancellationToken);
        return productId;
    }
}
