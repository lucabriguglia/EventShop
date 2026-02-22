# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build EventShop.sln

# Run component tests
dotnet test tests/EventShop.Tests.Component/EventShop.Tests.Component.csproj

# Run a single test by name
dotnet test tests/EventShop.Tests.Component/ --filter "FullyQualifiedName~AddItemToCart_ShouldSucceed"

# Run the web app
dotnet run --project src/EventShop.Web/EventShop.Web.csproj

# Run the Azure Function
dotnet run --project src/EventShop.Function/EventShop.Function.csproj
```

## Architecture

EventShop is a .NET 9 showcase of **DDD, CQRS, and Event Sourcing** using the [Memoria](https://github.com/lucabriguglia/Memoria) library. It uses SQLite for persistence (local dev) with options for Cosmos DB, Redis, RabbitMQ, and Azure Service Bus in production.

### Project Structure

```
src/
  EventShop.Domain/        # Aggregates, domain events, stream/aggregate IDs
  EventShop.Application/   # Command/query records, validators, handlers
  EventShop.Infrastructure/ # Read model projections (records only)
  EventShop.Web/           # Blazor Server app + ASP.NET Core Identity + EF Core
  EventShop.Function/      # Azure Functions app (Service Bus subscriber)
tests/
  EventShop.Tests.Component/ # Component tests via WebApplicationFactory
```

### Domain Bounded Contexts

Four contexts, each with parallel structure in Domain and Application:
- **Catalog** — `Product` aggregate
- **Customers** — `Customer` aggregate
- **Inventory** — `InventoryItem` aggregate
- **Ordering** — `ShoppingCart` and `Order` aggregates

### Event Sourcing Pattern (via Memoria)

Aggregates extend `AggregateRoot`. To change state, call `Add(new SomethingHappened(...))` which both records the event and triggers `Apply()`. The `Apply<T>` override uses a switch expression to dispatch to private `Apply(SpecificEvent)` methods that mutate state.

```csharp
[AggregateType("Product")]
public class Product : AggregateRoot
{
    public override Type[] EventTypeFilter => [typeof(ProductCreated), typeof(ProductPriceChanged)];

    public Result ChangePrice(decimal newPrice)
    {
        Add(new ProductPriceChanged(ProductId, newPrice)); // records + applies
        return Result.Ok();
    }

    protected override bool Apply<T>(T domainEvent) => domainEvent switch
    {
        ProductCreated @event => Apply(@event),
        ProductPriceChanged @event => Apply(@event),
        _ => false
    };
}
```

### Stream IDs and Aggregate IDs

Events are stored in **streams** (one per entity/context boundary) and addressed by **aggregate ID**. Stream IDs use a `type:guid` format:

```csharp
public record CustomerStreamId(Guid CustomerId) : IStreamId
{
    public string Id => $"customer:{CustomerId}";
}
```

`IDomainService` is used in handlers to load and save aggregates:
```csharp
var result = await domainService.GetAggregate(streamId, aggregateId);
await domainService.SaveAggregate(streamId, aggregateId, aggregate, expectedEventSequence: latestSeq);
```

### Command/Query Pattern

Commands, validators, and handlers are colocated in a single file in `Application/<Context>/Commands/`:

```csharp
public record CreateProduct(string Name, string Description, decimal Price) : ICommand<Guid>;

public class CreateProductValidator : AbstractValidator<CreateProduct> { ... }

public class CreateProductHandler(IDomainService domainService) : ICommandHandler<CreateProduct, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProduct command, CancellationToken cancellationToken = default) { ... }
}
```

Dispatch via `IDispatcher.Send(command)`. Pass `validateCommand: true` to trigger FluentValidation before handling.

### Results Pattern

All operations return `Result` or `Result<T>` from `Memoria.Results`. Check `result.IsSuccess` / `result.IsNotSuccess`. Failures carry `ErrorCode`, `Title`, and `Description`.

### Read Models (Projections)

`EventShop.Infrastructure/Projections/` contains simple `record` types representing read model shapes. Query handlers (in Application) are responsible for populating them (currently mostly unimplemented stubs).

### Testing Approach

Component tests use `WebApplicationFactory<Program>` with real command dispatch. The `IDomainService` is replaced with one backed by `EF Core InMemory` database. Tests inherit from `ComponentTestBase` which wires up `IDispatcher` and exposes `DomainService` for state verification.

Tests use xUnit + FluentAssertions (with `AssertionScope` for grouped assertions) + NSubstitute.

### Memoria Registration (Program.cs)

```csharp
builder.Services.AddMemoria(typeof(CreateProduct));           // scans assembly for commands/queries
builder.Services.AddMemoriaEventSourcing(typeof(Product));   // scans assembly for aggregates
builder.Services.AddMemoriaEntityFrameworkCore<ApplicationDbContext>();
builder.Services.AddMemoriaFluentValidation(typeof(CreateProduct));
```

The type argument is an anchor type for assembly scanning — any type from that assembly works.