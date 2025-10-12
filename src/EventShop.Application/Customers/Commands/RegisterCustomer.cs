using EventShop.Domain.Customers.Aggregates;
using EventShop.Domain.Streams;
using FluentValidation;
using Memoria.Commands;
using Memoria.EventSourcing;
using Memoria.Results;

namespace EventShop.Application.Customers.Commands;

public record RegisterCustomer(string Name) : ICommand<Guid>;

public class CreateCustomerValidator : AbstractValidator<RegisterCustomer>
{
    public CreateCustomerValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
    }
}

public class RegisterCustomerHandler(IDomainService domainService) : ICommandHandler<RegisterCustomer, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterCustomer command, CancellationToken cancellationToken = default)
    {
        var customerId = Guid.NewGuid();
        var streamId = new CustomerStreamId(customerId);
        var aggregateId = new CustomerAggregateId(customerId);
        var aggregate = new Customer(customerId, command.Name);
        await domainService.SaveAggregate(streamId, aggregateId, aggregate, expectedEventSequence: 0, cancellationToken);
        return customerId;
    }
}
