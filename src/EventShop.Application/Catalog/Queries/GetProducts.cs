using EventShop.Infrastructure.Projections;
using Memoria.Queries;
using Memoria.Results;

namespace EventShop.Application.Catalog.Queries;

public record GetProducts : IQuery<Product[]>;

public class GetProductsHandler : IQueryHandler<GetProducts, Product[]>
{
    public Task<Result<Product[]>> Handle(GetProducts query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
