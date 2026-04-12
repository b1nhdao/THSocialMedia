# Event Handler Not Being Triggered - SOLUTION

## Problem
After creating a post, the `PostCreatedEventHandler` breakpoint was not being hit. MongoDB showed connection activity but no data was inserted.

## Root Cause
**MediatR was only scanning the Application assembly for notification handlers**, but the event handlers (`PostCreatedEventHandler`, `PostUpdatedEventHandler`, `PostDeletedEventHandler`) are located in the **Infrastructure assembly**.

MediatR scans assemblies to discover:
- `IRequestHandler<>` implementations (Commands)
- `IRequestHandler<,>` implementations (Queries)  
- `INotificationHandler<>` implementations (Event Handlers)

Without scanning the Infrastructure assembly, the event handlers were never registered in the dependency injection container, so when events were published, there was no handler to invoke.

## Solution Applied

### 1. Updated ApplicationServices.cs
Modified the MediatR registration to scan both Application **and** Infrastructure assemblies:

**Before:**
```csharp
services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationServices)))
);
```

**After:**
```csharp
services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationServices))!);
    cfg.RegisterServicesFromAssembly(typeof(InfrastructureServices).Assembly);
});
```

This ensures MediatR discovers all:
- Command handlers (Application)
- Query handlers (Application)
- **Event handlers (Infrastructure)** ? This was missing!

### 2. Added Missing Using Statements
Added `using MediatR;` to event handler files for proper `INotificationHandler<>` implementation.

## How It Works Now

### Event Publishing Flow (with fix):

```
1. User creates post
   ?
2. CreatePostCommandHandler executes
   ?? Saves to PostgreSQL ?
   ?? Creates PostCreatedEvent object ?
   ?? Calls _eventBus.PublishEventAsync(postCreatedEvent) ?

3. InMemoryEventBus.PublishEventAsync() calls:
   ?? _mediator.Publish(domainEvent) ?

4. MediatR finds all INotificationHandler<PostCreatedEvent> implementations
   ?? Finds PostCreatedEventHandler (NOW REGISTERED!) ?
   ?? Invokes Handler.Handle() method ?

5. PostCreatedEventHandler.Handle() executes:
   ?? Gets user info ?
   ?? Creates PostReadModel ?
   ?? Inserts into MongoDB ?
```

## Verification Steps

### Step 1: Clean and Rebuild
```bash
cd C:\Users\Admin\source\repos\THSocialMedia
dotnet clean
dotnet build
```

### Step 2: Set Breakpoint
1. Open `THSocialMedia.Infrastructure\EventHandlers\PostCreatedEventHandler.cs`
2. Set breakpoint at the beginning of `Handle()` method (line 25)
3. Run application in Debug mode

### Step 3: Create a Post
Make a POST request to create a post:
```bash
curl -X POST http://localhost:5000/api/posts/create \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {your_token}" \
  -d '{
    "content": "Test post to trigger event handler",
    "visibility": 1,
    "fileUrls": null
  }'
```

### Step 4: Verify Breakpoint is Hit
? **Expected**: Breakpoint should now be hit in `PostCreatedEventHandler.Handle()`

### Step 5: Verify MongoDB Insert
Check MongoDB to confirm data was inserted:

**Using mongosh:**
```javascript
mongosh

use THSocialMediaRead

db.Posts.find()
```

**Using MongoDB Compass:**
1. Open MongoDB Compass
2. Navigate to `THSocialMediaRead` database
3. Look in `Posts` collection
4. Should see the newly created post document

**Using Postman:**
```bash
GET /api/posts/{postId}
Authorization: Bearer {token}
```

Should return the post with `UserName` and `UserAvatar` from MongoDB.

## What Was Fixed

### File: THSocialMedia.Application\ApplicationServices.cs
- ? Added Infrastructure assembly scanning to MediatR
- ? Added using statement for InfrastructureServices
- ? Now discovers event handlers from Infrastructure project

### File: THSocialMedia.Infrastructure\EventBus\InMemoryEventBus.cs
- ? Added missing `using MediatR;`

### File: THSocialMedia.Infrastructure\EventHandlers\PostCreatedEventHandler.cs
- ? Added missing `using MediatR;`

### File: THSocialMedia.Infrastructure\EventHandlers\PostUpdatedEventHandler.cs
- ? Added missing `using MediatR;`

### File: THSocialMedia.Infrastructure\EventHandlers\PostDeletedEventHandler.cs
- ? Added missing `using MediatR;`

## Testing Checklist

After applying the fix, verify:

- [ ] Application builds successfully
- [ ] Set breakpoint in PostCreatedEventHandler.Handle()
- [ ] Create a post via API
- [ ] Breakpoint is hit ?
- [ ] Post exists in PostgreSQL ?
- [ ] Post exists in MongoDB ?
- [ ] Breakpoint in PostUpdatedEventHandler works for updates ?
- [ ] Breakpoint in PostDeletedEventHandler works for deletes ?
- [ ] No exceptions in event handlers ?

## Common Issues After Fix

### Issue: Still not hitting breakpoint
**Check:**
1. Are you running in Debug mode?
2. Is breakpoint enabled (not disabled)?
3. Did you rebuild after code change?
4. Check Application Output window for errors
5. Check that Infrastructure project is referenced correctly

### Issue: MongoDB still empty
**Check:**
1. MongoDB is running (mongosh works?)
2. Connection string is correct
3. Network can reach MongoDB (firewall?)
4. Check application logs for handler errors
5. User data exists in PostgreSQL (needed for PostReadModel.UserName)

### Issue: Handler throws exception
**Check:**
1. View Application logs for exception details
2. Verify IPostReadRepository is registered
3. Verify IUserRepository is registered
4. Check MongoDB database/collection exists

## Performance Notes

- Event publishing is **asynchronous** but **awaited** in handlers
- No data loss if MongoDB is temporarily down (command still succeeds)
- Event handlers run **synchronously** with the request
- For high volume, consider async handler execution

## Related Files
- ApplicationServices.cs: MediatR configuration
- InMemoryEventBus.cs: Event publishing
- PostCreatedEventHandler.cs: Event handling
- CreatePostCommandHandler.cs: Event publishing trigger

## Next Steps

1. ? Apply the fix (already done)
2. ? Build solution
3. ? Test with breakpoints
4. ? Verify MongoDB data
5. Consider: Add more comprehensive logging
6. Consider: Implement event persistence for reliability
7. Consider: Add distributed event bus for scalability
