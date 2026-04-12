# Quick Start Guide - CQRS with MongoDB

## Prerequisites

1. **MongoDB** installed and running (local or Atlas)
2. **PostgreSQL** already configured
3. **.NET 9** SDK
4. **Visual Studio** or **VS Code**

## Setup Steps

### 1. Configure MongoDB

#### Option A: Local MongoDB
```bash
# Windows
# Download from https://www.mongodb.com/try/download/community
# Run installer and ensure MongoDB service is running

# Verify MongoDB is running
mongosh
```

#### Option B: MongoDB Atlas (Cloud)
```
1. Go to https://www.mongodb.com/cloud/atlas
2. Create account and free cluster
3. Get connection string
4. Update appsettings.json with your connection string
```

### 2. Update Configuration

Edit `appsettings.json` in `THSocialMedia.Api` project:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"  // or your MongoDB Atlas URI
  },
  "MongoDB": {
    "DatabaseName": "THSocialMediaRead"
  }
}
```

### 3. Build and Run

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
cd THSocialMedia.Api
dotnet run
```

The API will start and automatically:
- Create MongoDB database if it doesn't exist
- Create `Posts` collection
- Create index on `UserId`

## Usage Examples

### Create Post (Write to PostgreSQL + Sync to MongoDB)

```http
POST /api/posts/create
Content-Type: application/json
Authorization: Bearer {token}

{
  "content": "Hello World!",
  "visibility": 1,
  "fileUrls": null
}
```

**Response:**
```json
{
  "isSuccess": true,
  "value": "550e8400-e29b-41d4-a716-446655440000",
  "status": 200
}
```

### Get Post (Read from MongoDB)

```http
GET /api/posts/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer {token}
```

**Response:**
```json
{
  "isSuccess": true,
  "value": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "user-id",
    "content": "Hello World!",
    "visibility": 1,
    "fileUrls": null,
    "createdAt": "2024-12-19T10:30:00Z",
    "isDeleted": false,
    "commentsCount": 0,
    "comments": [],
    "reactionCounts": [
      {
        "type": 1,
        "name": "Like",
        "count": 0
      }
    ],
    "reactionViewModels": []
  },
  "status": 200
}
```

### Get All Posts (Read from MongoDB)

```http
GET /api/posts
Authorization: Bearer {token}
```

### Update Post (Write to PostgreSQL + Sync to MongoDB)

```http
PUT /api/posts/update/550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json
Authorization: Bearer {token}

{
  "content": "Updated content!",
  "visibility": 1
}
```

### Delete Post (Delete from PostgreSQL + MongoDB)

```http
DELETE /api/posts/delete/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer {token}
```

## Monitoring & Debugging

### Check MongoDB Data

Using MongoDB Compass or mongosh:

```javascript
// Connect to MongoDB
mongosh

// Switch to database
use THSocialMediaRead

// View posts collection
db.Posts.find()

// View specific post
db.Posts.findOne({ _id: ObjectId("...") })

// Check indexes
db.Posts.getIndexes()

// Count posts
db.Posts.countDocuments()
```

### View Logs

Check application logs for:
- Event publishing confirmation: `Post {PostId} synced to MongoDB read database`
- Event handler errors
- MongoDB connection issues

```
[INF] Post 550e8400-e29b-41d4-a716-446655440000 synced to MongoDB read database
```

## Verify CQRS is Working

### Step 1: Create a Post

```bash
curl -X POST http://localhost:5000/api/posts/create \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"content":"Test","visibility":1}'
```

Note the returned `postId`.

### Step 2: Query PostgreSQL

Check that post exists in PostgreSQL:
```sql
SELECT * FROM "Posts" WHERE "Id" = '{postId}';
```

### Step 3: Query MongoDB

Check that post exists in MongoDB:
```javascript
db.Posts.findOne({ Id: "{postId}" })
```

If both exist ? **CQRS is working!** ?

### Step 4: Get Via API (From MongoDB)

```bash
curl -X GET http://localhost:5000/api/posts/{postId} \
  -H "Authorization: Bearer {token}"
```

Response includes `UserName` and `UserAvatar` from MongoDB read model.

## Common Issues & Solutions

### Issue: "Cannot connect to MongoDB"
**Solution:**
1. Verify MongoDB is running: `mongosh`
2. Check connection string in appsettings.json
3. Check firewall if using remote MongoDB
4. Check MongoDB credentials if using authentication

### Issue: "Event handler not triggered"
**Solution:**
1. Verify post is in PostgreSQL (command executed)
2. Check application logs for handler exceptions
3. Verify MediatR is configured in ApplicationServices
4. Ensure Infrastructure project is referenced

### Issue: "MongoDB timeout"
**Solution:**
1. Check MongoDB is accessible
2. Increase connection timeout in settings
3. Check network connectivity
4. Verify correct connection string format

### Issue: "Index creation failed"
**Solution:**
1. Drop and recreate collection: `db.Posts.drop()`
2. Restart application to recreate indexes
3. Check MongoDB user has permission to create indexes

## Performance Tips

1. **Add Redis caching** on top of MongoDB queries
   ```csharp
   var cachedPost = await _cacheService.GetAsync<PostViewModel>(key);
   if (cachedPost == null)
   {
       cachedPost = await _postReadRepository.GetPostByIdAsync(id);
       await _cacheService.SetAsync(key, cachedPost);
   }
   ```

2. **Add MongoDB projections** for specific queries
   ```csharp
   // Get only specific fields
   var filter = Builders<PostReadModel>.Filter.Eq(x => x.UserId, userId);
   var projection = Builders<PostReadModel>.Projection
       .Include(x => x.Id)
       .Include(x => x.Content)
       .Include(x => x.CreatedAt);
   ```

3. **Batch insert operations** for bulk data
   ```csharp
   await _collection.InsertManyAsync(posts);
   ```

4. **Add compound indexes** for common queries
   ```csharp
   var indexKeysDefinition = Builders<PostReadModel>.IndexKeys
       .Ascending(x => x.UserId)
       .Descending(x => x.CreatedAt);
   ```

## What's Next?

1. **Implement CQRS for other entities** (User, Comment, Reaction)
2. **Add event persistence** for event sourcing
3. **Setup distributed event bus** (RabbitMQ, Service Bus)
4. **Implement read model projections** for complex queries
5. **Add comprehensive CQRS tests**
6. **Monitor and optimize** based on usage patterns

## Documentation Files

- **CQRS_IMPLEMENTATION.md**: Complete architecture documentation
- **IMPLEMENTATION_COMPLETE.md**: Detailed summary of all changes
- **CONTROLLER_EXAMPLE.cs**: Example API controller with CQRS usage
- **appsettings.example.json**: Configuration template

## Support

For issues or questions:
1. Check application logs
2. Review CQRS documentation
3. Verify MongoDB and PostgreSQL connectivity
4. Check handler registrations in DI
5. Review event publishing in command handlers
