# Domain-Driven Design & Clean Architecture Refactoring Plan

## Project Structure

```
FF.Articles.Backend.Contents/
├── FF.Articles.Backend.Contents.Domain/            # Core domain project - No external dependencies
│   ├── Aggregates/
│   │   ├── Articles/                              # Article aggregate root and related entities
│   │   │   ├── Article.cs                         # Article aggregate root
│   │   │   ├── Content.cs                         # Content value object
│   │   │   └── ArticleStatus.cs                   # Article status enumeration/value object
│   │   ├── Topics/                                # Topic aggregate and related entities
│   │   │   ├── Topic.cs                          # Topic aggregate root
│   │   │   └── TopicHierarchy.cs                 # Value object for topic relationships
│   │   └── Tags/                                  # Tag aggregate and related entities
│   │       └── Tag.cs                            # Tag entity
│   ├── Events/                                    # Domain events
│   │   ├── Articles/
│   │   │   ├── ArticleCreatedEvent.cs
│   │   │   ├── ContentUpdatedEvent.cs
│   │   │   └── ArticlePublishedEvent.cs
│   │   └── Topics/
│   │       └── TopicCreatedEvent.cs
│   ├── ValueObjects/                              # Shared value objects
│   │   ├── Title.cs
│   │   ├── Abstract.cs
│   │   └── Metadata.cs
│   ├── Exceptions/                                # Domain-specific exceptions
│   │   └── DomainException.cs
│   └── Interfaces/                                # Core interfaces
│       └── IRepository.cs
│
├── FF.Articles.Backend.Contents.Application/       # Application services and use cases
│   ├── UseCases/
│   │   ├── Articles/
│   │   │   ├── Commands/                          # Write operations
│   │   │   │   ├── CreateArticle/
│   │   │   │   │   ├── CreateArticleCommand.cs
│   │   │   │   │   └── CreateArticleHandler.cs
│   │   │   │   └── UpdateArticle/
│   │   │   │       ├── UpdateArticleCommand.cs
│   │   │   │       └── UpdateArticleHandler.cs
│   │   │   └── Queries/                           # Read operations
│   │   │       ├── GetArticle/
│   │   │       │   ├── GetArticleQuery.cs
│   │   │       │   └── GetArticleHandler.cs
│   │   │       └── SearchArticles/
│   │   │           ├── SearchArticlesQuery.cs
│   │   │           └── SearchArticlesHandler.cs
│   │   └── Topics/
│   │       └── [Similar structure for topics]
│   ├── DTOs/                                      # Data transfer objects
│   │   ├── ArticleDto.cs
│   │   └── TopicDto.cs
│   ├── Interfaces/                                # Application service interfaces
│   │   ├── IArticleRepository.cs
│   │   └── ITopicRepository.cs
│   └── Behaviors/                                 # Cross-cutting behaviors
│       ├── ValidationBehavior.cs
│       └── LoggingBehavior.cs
│
├── FF.Articles.Backend.Contents.Infrastructure/    # Infrastructure implementations
│   ├── Persistence/
│   │   ├── EntityFramework/
│   │   │   ├── ContentsDbContext.cs
│   │   │   ├── Configurations/                    # EF Core configurations
│   │   │   │   ├── ArticleConfiguration.cs
│   │   │   │   └── TopicConfiguration.cs
│   │   │   └── Repositories/
│   │   │       ├── ArticleRepository.cs
│   │   │       └── TopicRepository.cs
│   │   └── Elasticsearch/
│   │       └── ArticleSearchRepository.cs
│   ├── ExternalServices/
│   │   ├── Identity/
│   │   │   └── IdentityServiceAdapter.cs
│   │   └── MessageBroker/
│   │       └── RabbitMQAdapter.cs
│   ├── Caching/
│   │   └── RedisCacheService.cs
│   └── DependencyInjection/
│       └── ServiceCollectionExtensions.cs
│
└── FF.Articles.Backend.Contents.API/              # API layer
    ├── Controllers/
    │   ├── ArticlesController.cs
    │   └── TopicsController.cs
    ├── Middleware/
    │   ├── ExceptionHandlingMiddleware.cs
    │   └── RequestLoggingMiddleware.cs
    ├── Models/                                    # API models
    │   ├── Requests/
    │   │   ├── CreateArticleRequest.cs
    │   │   └── UpdateArticleRequest.cs
    │   └── Responses/
    │       └── ArticleResponse.cs
    ├── Validators/                                # Request validators
    │   └── CreateArticleRequestValidator.cs
    ├── Program.cs
    └── appsettings.json
```

## Project Responsibilities

### 1. FF.Articles.Backend.Contents.Domain
- **Purpose**: Core business logic and rules
- **Key Responsibilities**:
  - Domain entities and aggregates
  - Business rules and invariants
  - Domain events
  - No external dependencies
  - Pure C# code only
- **Key Components**:
  - Rich domain models with behavior
  - Value objects for immutable concepts
  - Domain events for state changes
  - Domain-specific exceptions

### 2. FF.Articles.Backend.Contents.Application
- **Purpose**: Orchestration of use cases
- **Key Responsibilities**:
  - Use case implementation
  - Command and query handlers
  - Application-level validation
  - Coordination between aggregates
- **Key Components**:
  - CQRS implementation
  - DTOs for data transfer
  - Interface definitions
  - Application services
  - No direct dependency on infrastructure

### 3. FF.Articles.Backend.Contents.Infrastructure
- **Purpose**: Technical implementations
- **Key Responsibilities**:
  - Data persistence
  - External service integration
  - Caching implementation
  - Message broker integration
  - Search functionality
- **Key Components**:
  - Repository implementations
  - Database contexts
  - External service adapters
  - Caching mechanisms
  - Search engine integration

### 4. FF.Articles.Backend.Contents.API
- **Purpose**: HTTP API and presentation
- **Key Responsibilities**:
  - API endpoints
  - Request/response models
  - Input validation
  - Authentication/Authorization
  - API documentation
- **Key Components**:
  - Controllers
  - API models
  - Middleware
  - Swagger documentation
  - Health checks

## Dependencies Flow
```
API → Application → Domain
↓
Infrastructure → Application
```

## Key Files Explanation

### Domain Layer
- `Article.cs`: Core article aggregate root with business rules
- `Topic.cs`: Topic aggregate managing hierarchical structure
- `DomainException.cs`: Custom exceptions for domain rule violations

### Application Layer
- `CreateArticleCommand.cs`: Command for article creation
- `GetArticleQuery.cs`: Query for article retrieval
- `IArticleRepository.cs`: Repository interface for article persistence

### Infrastructure Layer
- `ContentsDbContext.cs`: EF Core database context
- `ArticleRepository.cs`: Implementation of article repository
- `RabbitMQAdapter.cs`: Message broker integration

### API Layer
- `ArticlesController.cs`: REST API endpoints for articles
- `ExceptionHandlingMiddleware.cs`: Global exception handling
- `CreateArticleRequest.cs`: API request models

## Implementation Notes
1. Each project should have its own `.csproj` file
2. Use `Directory.Build.props` for shared project properties
3. Consider adding a shared kernel project for common components
4. Use solution folders to organize projects logically
5. Keep cross-project dependencies minimal and well-defined

## Current Codebase Analysis

### Existing Structure
- Traditional N-tier architecture
- Large service classes with mixed responsibilities (e.g., ArticleService handling CRUD, tags, and user operations)
- Direct dependency on external services in domain logic
- Anemic domain models (business logic in services rather than entities)
- Mixed concerns in services (data access, business rules, and external service calls)

### Key Components Identified
1. **Articles Management**
   - CRUD operations
   - Tag management
   - Topic associations
   - Parent-child relationships (SubArticles)
   - Content management

2. **Topics Management**
   - Topic hierarchies
   - Article categorization

3. **Tags Management**
   - Tag CRUD
   - Article-Tag associations

## Refactoring Priorities

### Phase 1: Domain Model Redesign
1. **Article Aggregate**
   ```csharp
   public class Article : AggregateRoot
   {
       public string Title { get; private set; }
       public string Content { get; private set; }
       public string Abstract { get; private set; }
       private readonly List<Tag> _tags;
       private readonly List<Article> _subArticles;
       
       public void UpdateContent(string content)
       {
           // Domain validation
           // Event raising
       }
       
       public void AddTag(Tag tag)
       {
           // Business rules
       }
   }
   ```

2. **Topic Aggregate**
   ```csharp
   public class Topic : AggregateRoot
   {
       public string Title { get; private set; }
       public Topic Parent { get; private set; }
       private readonly List<Topic> _subTopics;
       
       public void AddSubTopic(Topic topic)
       {
           // Validation and business rules
       }
   }
   ```

### Phase 2: Application Layer Implementation

1. **Use Cases**
   ```csharp
   public class CreateArticleUseCase
   {
       private readonly IArticleRepository _articleRepo;
       private readonly ITopicRepository _topicRepo;
       private readonly IUnitOfWork _unitOfWork;
       
       public async Task<Result<ArticleDto>> Execute(CreateArticleCommand command)
       {
           // Application logic
       }
   }
   ```

2. **Commands/Queries**
   ```csharp
   public record CreateArticleCommand(
       string Title,
       string Content,
       long TopicId,
       List<string> Tags,
       long UserId);
   
   public record GetArticleQuery(
       long ArticleId,
       bool IncludeContent,
       bool IncludeUser);
   ```

### Phase 3: Infrastructure Implementation

1. **Repository Implementations**
   ```csharp
   public class EfArticleRepository : IArticleRepository
   {
       private readonly ContentsDbContext _context;
       
       public async Task<Article> GetByIdWithTagsAsync(long id)
       {
           // Implementation
       }
   }
   ```

2. **External Service Adapters**
   ```csharp
   public class IdentityServiceAdapter : IIdentityService
   {
       private readonly IIdentityRemoteService _remoteService;
       
       public async Task<User> GetUserAsync(long userId)
       {
           // Adaptation layer
       }
   }
   ```

## Migration Steps

### 1. Initial Setup
- [ ] Create new project structure
- [ ] Set up domain project
- [ ] Configure dependency injection
- [ ] Create base classes (Entity, AggregateRoot, etc.)

### 2. Domain Layer 
- [ ] Implement Article aggregate
- [ ] Implement Topic aggregate
- [ ] Define domain events
- [ ] Create value objects
- [ ] Write domain services

### 3. Application Layer
- [ ] Implement CQRS infrastructure
- [ ] Create use cases for Articles
- [ ] Create use cases for Topics
- [ ] Implement validation

### 4. Infrastructure Layer
- [ ] Implement EF Core repositories
- [ ] Set up Elasticsearch integration
- [ ] Implement message broker integration
- [ ] Create external service adapters

### 5. API Layer
- [ ] Create new API controllers
- [ ] Implement request/response models
- [ ] Set up API versioning
- [ ] Add authentication/authorization

### 6. Migration
- [ ] Migrate existing data
- [ ] Run parallel systems
- [ ] Gradually switch traffic
- [ ] Remove old code

## Key Refactoring Points

1. **Split ArticleService**
   - Move article creation logic to CreateArticleUseCase
   - Move search logic to SearchArticlesUseCase
   - Move tag management to separate aggregate

2. **Improve Domain Model**
   - Make Article an aggregate root
   - Encapsulate business rules in entities
   - Use value objects for Title, Content, etc.

3. **Implement CQRS**
   - Separate read and write operations
   - Optimize read models for queries
   - Use commands for write operations

4. **Add Domain Events**
   - ArticleCreated
   - ContentUpdated
   - TagsChanged
   - TopicAssigned


## Monitoring and Metrics

1. **Performance Metrics**
   - Article creation time
   - Search response time
   - Database query times

2. **Business Metrics**
   - Articles created per day
   - Search queries per minute
   - Tag usage statistics

## Risk Mitigation

1. **Data Migration**
   - Create data migration scripts
   - Test with production data copy
   - Plan rollback strategy

2. **Performance**
   - Benchmark current performance
   - Set performance targets
   - Monitor during migration

3. **Business Continuity**
   - Run systems in parallel
   - Feature flags for new implementation
   - Gradual traffic migration
