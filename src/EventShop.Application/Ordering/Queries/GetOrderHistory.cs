using EventShop.Infrastructure.Projections;
using Memoria.Queries;

namespace EventShop.Application.Ordering.Queries;

public record GetOrderHistory(Guid CustomerId) : IQuery<List<Order>>;

public class GetOrderHistoryHandler
{
    public Task<List<Order>> Handle(GetOrderHistory query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
