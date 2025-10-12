using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Streams;

public record CustomerStreamId(Guid CustomerId) : IStreamId
{
    public string Id => $"customer:{CustomerId}";
}
