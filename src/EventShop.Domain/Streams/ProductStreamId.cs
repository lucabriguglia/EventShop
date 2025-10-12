using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Streams;

public record ProductStreamId(Guid ProductId) : IStreamId
{
    public string Id => $"product:{ProductId}";
}
