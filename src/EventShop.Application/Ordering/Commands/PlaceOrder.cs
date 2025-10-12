using Memoria.Commands;
using Memoria.Results;

namespace EventShop.Application.Ordering.Commands;

public record PlaceOrder(Guid CustomerId, List<OrderItem> Items) : ICommand<Guid>;
public record OrderItem(Guid ProductId, int Quantity, decimal Price);

public class PlaceOrderHandler : ICommandHandler<PlaceOrder, Guid>
{
    public Task<Result<Guid>> Handle(PlaceOrder command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
