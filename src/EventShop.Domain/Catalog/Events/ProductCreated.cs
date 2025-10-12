using Memoria.EventSourcing.Domain;

namespace EventShop.Domain.Catalog.Events;

[EventType("ProductCreated")]
public record ProductCreated(Guid ProductId, string Name, string Description, decimal Price) : IEvent;
