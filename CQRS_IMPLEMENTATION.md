# CQRS Implementation with MongoDB Read Database

## Overview

This document describes the full CQRS (Command Query Responsibility Segregation) implementation with MongoDB as a read database and event-driven synchronization for your THSocialMedia application.

## Architecture

### Components Implemented

#### 1. **Domain Layer** (THSocialMedia.Domain)
- **IDomainEvent**: Base interface for all domain events
- **IEventBus**: Interface for publishing domain events
- **Domain Events**:
  - `PostCreatedEvent`: Published when a post is created
  - `PostUpdatedEvent`: Published when a post is updated
  - `PostDeletedEvent`: Published when a post is deleted

All events implement both `IDomainEvent` and MediatR's `INotification` for seamless integration with the CQRS pattern.

#### 2. **Application Layer** (THSocialMedia.Application)
- **Commands** (Write Operations):
  - `CreatePostCommand` ? `CreatePostCommandHandler`
  - `UpdatePostCommand` ? `UpdatePostCommandHandler`
  - `DeletePostCommand` ? `DeletePostCommandHandler`
  - Each handler publishes an event after successful command execution

- **Queries** (Read Operations - from MongoDB):
  - `GetPostByIdReadQuery` ? `GetPostByIdReadQueryHandler`
  - `GetAllPostsReadQuery` ? `GetAllPostsReadQueryHandler`
  - Both handlers query the MongoDB read database for fast, optimized reads

#### 3. **Infrastructure Layer** (THSocialMedia.Infrastructure)

##### MongoDB Configuration
- **ReadModels**: `PostReadModel` - Optimized read model for MongoDB
- **Repositories**:
  - `IPostReadRepository`: Interface for read operations
  - `PostReadRepository`: MongoDB implementation with:
    - `GetPostByIdAsync()`: Retrieve single post
    - `GetPostsByUserIdAsync()`: Retrieve user's posts
    - `GetAllPostsAsync()`: Retrieve all posts (sorted by creation date)
    - `CreatePostAsync()`: Sync new posts
    - `UpdatePostAsync()`: Sync post updates
    - `DeletePostAsync()`: Sync post deletions

##### Event Bus
- **InMemoryEventBus**: Implements `IEventBus` using MediatR
  - Publishes domain events as MediatR notifications
  - Automatically triggers registered event handlers

##### Event Handlers (Read Model Synchronization)
- `PostCreatedEventHandler`: Syncs new posts to MongoDB
- `PostUpdatedEventHandler`: Syncs post updates to MongoDB
- `PostDeletedEventHandler`: Syncs post deletions to MongoDB

## Data Flow

### Writing (Command Path)
```
1. User creates/updates/deletes post
    ?
2. Command Handler executes business logic
    ?
3. Data written to PostgreSQL (WriteDbContext)
    ?
4. Domain Event published via IEventBus
    ?
5. Event Handler receives event
    ?
6. Read model synchronized to MongoDB
```

### Reading (Query Path)
```
1. User requests post(s)
    ?
2. Query Handler executed
    ?
3. Data fetched directly from MongoDB
    ?
4. Optimized read model returned to client
```

## Configuration

### appsettings.json Setup

Add MongoDB connection string to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=THSocialMediaWrite;Username=postgres;Password=your_password",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379"
  },
  "MongoDB": {
    "DatabaseName": "THSocialMediaRead"
  }
}
```

### Dependency Injection

The `InfrastructureServices.AddInfrastructureServices()` extension method now registers:

```csharp
// MongoDB services
services.AddSingleton<IMongoClient>(...)
services.AddSingleton<IMongoDatabase>(...)

// Event Bus
services.AddScoped<IEventBus, InMemoryEventBus>();

// Read repositories
services.AddScoped<IPostReadRepository, PostReadRepository>();

// Command repositories and UoW
services.AddScoped<IPostRepository, PostRepository>();
services.AddScoped<IUnitOfWork>(...)
```

## NuGet Packages Added

- **MongoDB.Driver** (v2.27.0): For MongoDB operations
- **MediatR** (v14.1.0): For CQRS and event handling

Update Domain project csproj to include MediatR dependency.

## Usage Examples

### Writing Data (Command)

```csharp
var command = new CreatePostCommand
{
    Content = "Hello World",
    Visibility = 1,
    FileUrls = null
};

var result = await _mediator.Send(command);
// Event published automatically ? MongoDB synced

if (result.IsSuccess)
{
    var postId = result.Value;
}
```

### Reading Data (Query)

```csharp
// Get single post from MongoDB
var query = new GetPostByIdReadQuery(postId);
var result = await _mediator.Send(query);

if (result.IsSuccess)
{
    var postViewModel = result.Value;
}

// Get all posts from MongoDB
var allPostsQuery = new GetAllPostsReadQuery();
var allPostsResult = await _mediator.Send(allPostsQuery);
```

## Database Schemas

### PostgreSQL (Write Database - WriteDbContext)
- Maintains transactional integrity
- Contains complete Post entity with all relationships
- Used for write operations only

### MongoDB (Read Database)
- Collection: `Posts`
- Schema:
```json
{
  "_id": "ObjectId",
  "Id": "guid",
  "UserId": "guid",
  "UserName": "string",
  "UserAvatar": "string",
  "Content": "string",
  "Visibility": "int",
  "FileUrls": "string | null",
  "CreatedAt": "datetime",
  "UpdatedAt": "datetime",
  "ReactionsCount": "int",
  "CommentsCount": "int"
}
```

### Indexes
- Automatically created on `UserId` for faster user-specific queries
- Sorted queries default to `CreatedAt` descending (latest first)

## Error Handling

Event handlers include comprehensive error handling:
- Errors are logged but don't fail the command
- Posts are written to PostgreSQL even if MongoDB sync fails
- Graceful degradation ensures system resilience

## Future Enhancements

1. **Event Persistence**: Store events in an Event Store for replay capabilities
2. **Eventual Consistency**: Implement background jobs for retry on failed syncs
3. **Additional Read Models**: Create specialized views for different query patterns
4. **Event Versioning**: Handle schema evolution of domain events
5. **Distributed Event Bus**: Replace InMemoryEventBus with messaging infrastructure (RabbitMQ, Service Bus)
6. **Projections**: Build specialized MongoDB projections for different UI needs
7. **CQRS for Other Aggregates**: Apply same pattern to User, Comment, Reaction entities

## Testing

When testing:
- Mock `IEventBus` for unit tests of command handlers
- Mock `IPostReadRepository` for unit tests of query handlers
- Use integration tests with real MongoDB for event handler testing
- Verify events are published after successful commands
- Verify MongoDB data consistency after events

## Performance Considerations

- **Writes**: Accept slightly higher latency (PostgreSQL + Event Publishing)
- **Reads**: Dramatically improved with MongoDB read models and indexes
- **Scalability**: MongoDB can scale independently from PostgreSQL
- **Caching**: Add Redis layer on top of MongoDB for frequently accessed data
- **Projections**: Build custom MongoDB projections for specific query patterns

## Troubleshooting

### MongoDB Connection Issues
- Verify MongoDB is running on configured host/port
- Check connection string format
- Ensure database exists or will be auto-created

### Events Not Syncing
- Check event handler registrations in DI container
- Verify MediatR is configured to discover handlers
- Monitor application logs for event handler exceptions

### Read Model Out of Sync
- Check write handler logs for event publishing failures
- Verify MongoDB connectivity
- Consider implementing event replay for recovery
