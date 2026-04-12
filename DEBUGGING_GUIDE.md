# CQRS Debugging Guide

## Issue: Event Handler Not Triggering

### Quick Diagnosis

The breakpoint in `PostCreatedEventHandler` wasn't being hit because:

1. ? MediatR wasn't scanning Infrastructure assembly
2. ? Event handlers weren't registered in DI container
3. ? When event published, no handler found ? event discarded silently

### The Fix (Already Applied)

? **ApplicationServices.cs** now registers MediatR to scan **both** assemblies:

```csharp
services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationServices))!);
    cfg.RegisterServicesFromAssembly(typeof(InfrastructureServices).Assembly); // Added this
});
```

---

## Debugging Event Flow

### 1. Verify MediatR is Discovering Handlers

Add this to your startup or a test:

```csharp
// In Program.cs or test, after services configured
var scope = app.Services.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

// Try to get handler (if no exception, handler was found)
var notificationHandlers = app.Services.GetServices(
    typeof(INotificationHandler<PostCreatedEvent>)
);
Console.WriteLine($"Found {notificationHandlers.Count()} handlers for PostCreatedEvent");
```

**Expected Output:**
```
Found 1 handlers for PostCreatedEvent
```

### 2. Verify Event is Being Published

Add logging to `InMemoryEventBus.cs`:

```csharp
public async Task PublishEventAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
    where TEvent : IDomainEvent
{
    _logger.LogInformation("Publishing event: {EventType} for aggregate {AggregateId}", 
        domainEvent.EventType, domainEvent.AggregateId);

    await _mediator.Publish(domainEvent, cancellationToken);

    _logger.LogInformation("Event published successfully: {EventType}", 
        domainEvent.EventType);
}
```

**Expected Logs:**
```
Publishing event: PostCreatedEvent for aggregate 550e8400-e29b-41d4-a716-446655440000
Event published successfully: PostCreatedEvent
```

### 3. Verify Handler is Invoked

Breakpoint locations to check:

1. **CreatePostCommandHandler.cs - Line 88** (Event publishing)
   - Verify event object is created correctly
   - Verify post ID is valid

2. **InMemoryEventBus.cs - Line 18** (Event publishing via MediatR)
   - Verify _mediator.Publish is called
   - Check for exceptions

3. **PostCreatedEventHandler.cs - Line 25** (Handle method entry)
   - ? **SHOULD HIT NOW** after the fix
   - Verify notification parameter has correct data

4. **PostReadRepository.cs - Line 49** (MongoDB insert)
   - Verify PostReadModel created correctly
   - Check for MongoDB exceptions

### 4. Verify MongoDB Insert Success

After hitting PostCreatedEventHandler breakpoint:

```csharp
// In PostCreatedEventHandler.Handle()
_logger.LogInformation("Creating post read model: {PostId} for user {UserId}", 
    notification.PostId, notification.UserId);

await _postReadRepository.CreatePostAsync(postReadModel, cancellationToken);

_logger.LogInformation("Post read model created successfully: {PostId}", notification.PostId);
```

**Expected Logs:**
```
Creating post read model: 550e8400-e29b-41d4-a716-446655440000 for user user-id
Post read model created successfully: 550e8400-e29b-41d4-a716-446655440000
```

---

## MongoDB Verification

### Check if Data Was Actually Inserted

**Using mongosh:**
```javascript
// Connect
mongosh

// Switch database
use THSocialMediaRead

// Count posts
db.Posts.countDocuments()

// Find all posts
db.Posts.find()

// Find specific post
db.Posts.findOne({ Id: "550e8400-e29b-41d4-a716-446655440000" })

// Check indexes
db.Posts.getIndexes()
```

### Check MongoDB Logs

MongoDB logs often show insert failures:

```bash
# Windows - MongoDB logs in default location
type "C:\Program Files\MongoDB\Server\6.0\log\mongod.log" | tail -50

# Or in MongoDB Compass:
# Server ? Logs ? View
```

### Verify Connection String

```csharp
// In InfrastructureServices.cs
var mongoConnectionString = _configuration.GetConnectionString("MongoDB");
_logger.LogInformation("MongoDB Connection String: {ConnectionString}", mongoConnectionString);

// Verify format: mongodb://host:port or mongodb+srv://...
```

---

## Event Handler Execution Flow - Detailed

### Normal Flow (After Fix):

```
1. POST /api/posts/create
   ?
2. CreatePostCommandHandler.Handle()
   ?? Create Post object
   ?? Save to PostgreSQL (WriteDbContext.Posts.Add + SaveChanges)
   ?? Create PostCreatedEvent
   ?? Call _eventBus.PublishEventAsync(event)
   ?  ?? InMemoryEventBus.PublishEventAsync()
   ?     ?? _mediator.Publish(event)  ? Broadcasts to all handlers
   ?        ?? PostCreatedEventHandler.Handle() ? FOUND & INVOKED
   ?           ?? Get user from PostgreSQL
   ?           ?? Create PostReadModel
   ?           ?? Insert into MongoDB
   ?? Return post ID
   ?
3. Response: { postId: "..." }
```

### MongoDB Transaction Details:

```csharp
// In PostReadRepository.CreatePostAsync()
post.CreatedAt = DateTime.UtcNow;
post.UpdatedAt = DateTime.UtcNow;
await _collection.InsertOneAsync(post, null, cancellationToken);
//                                                    ?
//                                         Check this doesn't throw
```

---

## Common Problems & Solutions

### Problem 1: "No handler registered for INotificationHandler<PostCreatedEvent>"

**Cause:** ApplicationServices.cs not scanning Infrastructure assembly

**Solution:** 
```csharp
// ? CORRECT
services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationServices))!);
    cfg.RegisterServicesFromAssembly(typeof(InfrastructureServices).Assembly);
});

// ? WRONG (only scans Application)
services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationServices)))
);
```

### Problem 2: Handler throws MongoDB exception

**Check:**
```csharp
// In PostCreatedEventHandler.cs
var postReadModel = new PostReadModel { /* ... */ };
try 
{
    await _postReadRepository.CreatePostAsync(postReadModel, cancellationToken);
    // Check: Does MongoDB collection exist?
    // Check: Is connection string valid?
    // Check: Does database have write permissions?
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to insert to MongoDB");
    throw; // Handler will fail - check logs
}
```

### Problem 3: Handler times out on MongoDB

**Check:**
```csharp
// Connection timeout
var client = new MongoClient("mongodb://localhost:27017");
// Add connection settings:
// serverSelectionTimeoutMS, socketTimeoutMS, etc.
```

### Problem 4: User not found, so UserName is "Unknown"

**Reason:** User might not exist in PostgreSQL yet

**Verify:**
```sql
SELECT * FROM "Users" WHERE "Id" = '{userId}'
```

---

## Logging Configuration

Add detailed logging for debugging:

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "THSocialMedia": "Debug",
      "THSocialMedia.Infrastructure.EventHandlers": "Information",
      "THSocialMedia.Infrastructure.EventBus": "Information",
      "THSocialMedia.Infrastructure.MongoDb": "Information",
      "MongoDB.Driver": "Warning"
    }
  }
}
```

**In code:**
```csharp
_logger.LogInformation("Event handler processing event: {EventType}", notification.EventType);
_logger.LogDebug("Event details: {@Event}", notification);
_logger.LogInformation("Post synced successfully: {PostId}", postReadModel.Id);
```

---

## Test Checklist

- [ ] Application builds without errors
- [ ] MongoDB is running and accessible
- [ ] PostgreSQL is running and accessible
- [ ] Set breakpoint in CreatePostCommandHandler (line 88)
- [ ] Set breakpoint in PostCreatedEventHandler (line 25)
- [ ] Create post via API
- [ ] CreatePostCommandHandler breakpoint hit ?
- [ ] PostCreatedEventHandler breakpoint hit ? ? **THIS WAS THE ISSUE**
- [ ] Post in PostgreSQL ?
- [ ] Post in MongoDB ?
- [ ] No errors in Application Output ?

---

## Next: Verify the Fix Works

1. **Rebuild:** `dotnet build`
2. **Set Breakpoint:** PostCreatedEventHandler.cs line 25
3. **Create Post:** POST /api/posts/create
4. **Check:** Breakpoint should hit ?

If breakpoint still doesn't hit:
1. Check build output for compilation errors
2. Check Application Output for exceptions
3. Verify event is being published (add logging)
4. Check that Infrastructure project builds correctly

---

## Summary

**The Fix:** MediatR now scans Infrastructure assembly for event handlers

**Result:** Event handlers are discovered and registered in DI container

**Outcome:** When events published, handlers execute ? MongoDB synced ?
