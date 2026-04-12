# How to Add Event Handlers in the Future

## Context

Now that MediatR is scanning both Application and Infrastructure assemblies, any new event handlers added to Infrastructure will be automatically discovered.

## Adding a New Event Handler

### Step 1: Create the Domain Event

**File: `THSocialMedia.Domain\Events\YourNewEvent.cs`**

```csharp
using MediatR;
using THSocialMedia.Domain.Abstractions;

namespace THSocialMedia.Domain.Events
{
    public class YourNewEvent : IDomainEvent, INotification
    {
        public Guid AggregateId { get; }
        public DateTime CreatedAt { get; }
        public string EventType => nameof(YourNewEvent);

        public Guid EntityId { get; }
        public string SomeData { get; }

        public YourNewEvent(Guid entityId, string someData)
        {
            EntityId = entityId;
            AggregateId = entityId;
            SomeData = someData;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
```

### Step 2: Publish the Event in Your Command Handler

**File: `THSocialMedia.Application\UsecaseHandlers\YourFeature\Handlers\YourCommandHandler.cs`**

```csharp
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Events;

public class YourCommandHandler : IRequestHandler<YourCommand, Result<Guid>>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<YourCommandHandler> _logger;
    // ... other dependencies

    public YourCommandHandler(IEventBus eventBus, ILogger<YourCommandHandler> logger, ...)
    {
        _eventBus = eventBus;
        _logger = logger;
        // ...
    }

    public async Task<Result<Guid>> Handle(YourCommand request, CancellationToken cancellationToken)
    {
        // Your business logic
        var entity = new YourEntity { /* ... */ };

        // Save to database
        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event to trigger handlers
        try
        {
            var @event = new YourNewEvent(entity.Id, entity.SomeData);
            await _eventBus.PublishEventAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish YourNewEvent for entity {EntityId}", entity.Id);
        }

        return Result<Guid>.Success(entity.Id);
    }
}
```

### Step 3: Create the Event Handler

**File: `THSocialMedia.Infrastructure\EventHandlers\YourNewEventHandler.cs`**

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Domain.Events;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;

namespace THSocialMedia.Infrastructure.EventHandlers
{
    public class YourNewEventHandler : INotificationHandler<YourNewEvent>
    {
        private readonly IYourReadRepository _readRepository;
        private readonly ILogger<YourNewEventHandler> _logger;

        public YourNewEventHandler(
            IYourReadRepository readRepository,
            ILogger<YourNewEventHandler> logger)
        {
            _readRepository = readRepository;
            _logger = logger;
        }

        public async Task Handle(YourNewEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Create read model
                var readModel = new YourReadModel
                {
                    Id = notification.EntityId,
                    SomeData = notification.SomeData,
                    CreatedAt = notification.CreatedAt,
                    UpdatedAt = notification.CreatedAt
                };

                // Sync to MongoDB
                await _readRepository.CreateAsync(readModel, cancellationToken);

                _logger.LogInformation("Entity {EntityId} synced to read database", notification.EntityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing entity {EntityId}", notification.EntityId);
                throw;
            }
        }
    }
}
```

### Step 4: Create the Read Model

**File: `THSocialMedia.Infrastructure\MongoDb\ReadModels\YourReadModel.cs`**

```csharp
namespace THSocialMedia.Infrastructure.MongoDb.ReadModels
{
    public class YourReadModel
    {
        public Guid Id { get; set; }
        public string SomeData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

### Step 5: Create the Read Repository Interface

**File: `THSocialMedia.Infrastructure\MongoDb\Abstractions\IYourReadRepository.cs`**

```csharp
using THSocialMedia.Infrastructure.MongoDb.ReadModels;

namespace THSocialMedia.Infrastructure.MongoDb.Abstractions
{
    public interface IYourReadRepository
    {
        Task<YourReadModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<YourReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task CreateAsync(YourReadModel model, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, YourReadModel model, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
```

### Step 6: Create the Read Repository Implementation

**File: `THSocialMedia.Infrastructure\MongoDb\Repositories\YourReadRepository.cs`**

```csharp
using MongoDB.Driver;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;
using THSocialMedia.Infrastructure.MongoDb.ReadModels;

namespace THSocialMedia.Infrastructure.MongoDb.Repositories
{
    public class YourReadRepository : IYourReadRepository
    {
        private readonly IMongoCollection<YourReadModel> _collection;

        public YourReadRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<YourReadModel>("YourModels");
        }

        public async Task<YourReadModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<YourReadModel>.Filter.Eq(x => x.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<YourReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task CreateAsync(YourReadModel model, CancellationToken cancellationToken = default)
        {
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(model, null, cancellationToken);
        }

        public async Task UpdateAsync(Guid id, YourReadModel model, CancellationToken cancellationToken = default)
        {
            model.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<YourReadModel>.Filter.Eq(x => x.Id, id);
            await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<YourReadModel>.Filter.Eq(x => x.Id, id);
            await _collection.DeleteOneAsync(filter, null, cancellationToken);
        }
    }
}
```

### Step 7: Register in DI Container

**File: `THSocialMedia.Infrastructure\InfrastructureServices.cs`**

```csharp
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration _configuration)
{
    // ... existing registrations ...

    // Add your read repository
    services.AddScoped<IYourReadRepository, YourReadRepository>();

    return services;
}
```

### Step 8: Create Query (Optional - for reading from MongoDB)

**File: `THSocialMedia.Application\UsecaseHandlers\YourFeature\Queries\GetYourByIdReadQuery.cs`**

```csharp
using Ardalis.Result;
using MediatR;

namespace THSocialMedia.Application.UsecaseHandlers.YourFeature.Queries
{
    public class GetYourByIdReadQuery : IRequest<Result<YourViewModel>>
    {
        public Guid Id { get; set; }

        public GetYourByIdReadQuery(Guid id)
        {
            Id = id;
        }
    }
}
```

### Step 9: Create Query Handler

**File: `THSocialMedia.Application\UsecaseHandlers\YourFeature\Handlers\GetYourByIdReadQueryHandler.cs`**

```csharp
using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using THSocialMedia.Application.UsecaseHandlers.YourFeature.Queries;
using THSocialMedia.Application.UsecaseHandlers.YourFeature.ViewModels;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;

namespace THSocialMedia.Application.UsecaseHandlers.YourFeature.Handlers
{
    public class GetYourByIdReadQueryHandler : IRequestHandler<GetYourByIdReadQuery, Result<YourViewModel>>
    {
        private readonly IYourReadRepository _readRepository;
        private readonly ILogger<GetYourByIdReadQueryHandler> _logger;

        public GetYourByIdReadQueryHandler(
            IYourReadRepository readRepository,
            ILogger<GetYourByIdReadQueryHandler> logger)
        {
            _readRepository = readRepository;
            _logger = logger;
        }

        public async Task<Result<YourViewModel>> Handle(GetYourByIdReadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var readModel = await _readRepository.GetByIdAsync(request.Id, cancellationToken);

                if (readModel == null)
                    return Result.NotFound();

                var viewModel = new YourViewModel
                {
                    Id = readModel.Id,
                    SomeData = readModel.SomeData,
                    CreatedAt = readModel.CreatedAt
                };

                return Result.Success(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity {Id}", request.Id);
                return Result.Error($"Error: {ex.Message}");
            }
        }
    }
}
```

## Automatic Discovery

**No need to manually register anything!**

Because of the fix to `ApplicationServices.cs`, MediatR will automatically discover:
- ? `INotificationHandler<YourNewEvent>` implementations
- ? `IRequestHandler<GetYourByIdReadQuery, Result<YourViewModel>>` implementations

The event handler will be invoked automatically when the event is published.

## Testing Your New Event Handler

### Set Breakpoint
```csharp
// Line 1 of Handle() method
public async Task Handle(YourNewEvent notification, CancellationToken cancellationToken)
{  // ? Set breakpoint here
```

### Create Entity
```bash
POST /api/your/create
Authorization: Bearer {token}
Content-Type: application/json

{
  "someData": "test value"
}
```

### Verify
1. ? Event handler breakpoint hits
2. ? Data in PostgreSQL
3. ? Data in MongoDB

## Checklist for New Event Handler

- [ ] Domain Event created (implements IDomainEvent, INotification)
- [ ] Event published in command handler
- [ ] Event handler created (implements INotificationHandler<T>)
- [ ] Read model created
- [ ] Read repository interface created
- [ ] Read repository implementation created
- [ ] Read repository registered in DI (InfrastructureServices.cs)
- [ ] Query created (optional, for reading)
- [ ] Query handler created (optional, for reading)
- [ ] Build succeeds
- [ ] Breakpoint set in event handler
- [ ] Test: Create entity ? Breakpoint hits ?
- [ ] Test: Verify PostgreSQL has data ?
- [ ] Test: Verify MongoDB has data ?

## Key Points to Remember

1. **Event handlers are auto-discovered** - No manual registration needed
2. **Handlers run synchronously** - They await completion before responding
3. **MongoDB collection auto-created** - First write creates collection
4. **Errors don't block commands** - Handler exceptions are logged but don't fail the command
5. **Clean architecture** - Infrastructure knows about Infrastructure only, not vice versa

## Related Files

- Original implementation: See PostCreatedEvent/PostCreatedEventHandler for reference
- Event bus: InMemoryEventBus.cs
- Infrastructure registration: InfrastructureServices.cs
