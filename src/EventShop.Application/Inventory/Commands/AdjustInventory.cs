using Memoria.Commands;
using Memoria.Results;

namespace EventShop.Application.Inventory.Commands;

public record AdjustInventory(Guid ProductId, int Quantity) : ICommand;

public class AdjustInventoryHandler : ICommandHandler<AdjustInventory>
{
    public Task<Result> Handle(AdjustInventory command, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}
