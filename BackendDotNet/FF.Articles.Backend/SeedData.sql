
USE FireflyApp
GO
TRUNCATE TABLE users

INSERT INTO Users(UserAccount,UserPassword,UserName,UserProfile,UserRole,CreateTime,UpdateTime)
VALUES
('firefly', '0732d07230a94c9318ecdfc223dfb310', 'Firefly Admin', 'Website host', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
('test1', '0732d07230a94c9318ecdfc223dfb310', 'test Admin', 'developer', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
('test2', '0732d07230a94c9318ecdfc223dfb310', 'test Admin2', 'admin', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
('test3', '0732d07230a94c9318ecdfc223dfb310', 'test user', 'default user', 'user','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638');

TRUNCATE TABLE Contents.Tag
TRUNCATE TABLE Contents.Topic
TRUNCATE TABLE Contents.Article
TRUNCATE TABLE Contents.ArticleTag

-- Insert Programming Topics
INSERT INTO Contents.Topic (Title, Content, Abstraction, UserId, SortNumber, IsHidden, CreateTime, UpdateTime, IsDelete)
VALUES
    ('C#', 'Comprehensive resources about C# programming', 'C# language fundamentals and advanced concepts', 1, 1, 0, GETDATE(), GETDATE(), 0),
    ('Java', 'Java development guides and best practices', 'Enterprise Java development resources', 1, 2, 0, GETDATE(), GETDATE(), 0),
    ('React', 'Modern React development tutorials', 'Client-side rendering with React', 1, 3, 0, GETDATE(), GETDATE(), 0),
    ('EF Core', 'Entity Framework Core ORM documentation', 'Database access with EF Core', 1, 4, 0, GETDATE(), GETDATE(), 0),
    ('Blazor', 'Blazor component-based web development', 'C# web frameworks with WebAssembly', 1, 5, 0, GETDATE(), GETDATE(), 0),
    ('MAUI', 'Cross-platform development with .NET MAUI', 'Native mobile/desktop applications', 1, 6, 0, GETDATE(), GETDATE(), 0),
    ('SSR/CSR', 'Server-Side vs Client-Side Rendering', 'Rendering architecture comparisons', 1, 7, 0, GETDATE(), GETDATE(), 0);

-- Insert Articles for Each Topic
INSERT INTO Contents.Article (Title, Content, Abstraction, UserId, TopicId, SortNumber, IsHidden, CreateTime, UpdateTime, IsDelete)
VALUES
    -- C# Articles (TopicId = 1)
    ('C# 12 Pattern Matching', 'Deep dive into new pattern matching features...', 'Advanced C# techniques', 1, 1, 1, 0, GETDATE(), GETDATE(), 0),
    ('ASP.NET Core Dependency Injection', 'Implementing DI in C# web apps...', 'DI best practices', 1, 1, 2, 0, GETDATE(), GETDATE(), 0),
    ('C# Performance Optimization', 'Memory management and GC tuning...', 'High-performance C#', 1, 1, 3, 0, GETDATE(), GETDATE(), 0),

    -- Java Articles (TopicId = 2)
    ('Java Stream API Guide', 'Modern data processing with streams...', 'Functional programming in Java', 1, 2, 1, 0, GETDATE(), GETDATE(), 0),
    ('Spring Security Best Practices', 'Securing Java web applications...', 'Authentication patterns', 1, 2, 2, 0, GETDATE(), GETDATE(), 0),
    ('Java 21 Virtual Threads', 'Lightweight concurrency model...', 'Project Loom implementation', 1, 2, 3, 0, GETDATE(), GETDATE(), 0),

    -- React Articles (TopicId = 3)
    ('React Server Components', 'Next-gen React architecture...', 'Hybrid rendering patterns', 1, 3, 1, 0, GETDATE(), GETDATE(), 0),
    ('React TypeScript Setup', 'Configuring TS with Webpack...', 'Type-safe React development', 1, 3, 2, 0, GETDATE(), GETDATE(), 0),
    ('Testing React Components', 'Jest and React Testing Library...', 'Component testing strategies', 1, 3, 3, 0, GETDATE(), GETDATE(), 0),

    -- EF Core Articles (TopicId = 4)
    ('EF Core SQL Server', 'Optimizing SQL queries with EF...', 'Database performance tuning', 1, 4, 1, 0, GETDATE(), GETDATE(), 0),
    ('EF Core Relationships', 'Configuring 1:1 and 1:many...', 'Data modeling patterns', 1, 4, 2, 0, GETDATE(), GETDATE(), 0),
    ('EF Core Cosmos DB', 'NoSQL integration patterns...', 'Cloud database solutions', 1, 4, 3, 0, GETDATE(), GETDATE(), 0),

    -- Blazor Articles (TopicId = 5)
    ('Blazor Authentication', 'Implementing Auth0 integration...', 'Security in Blazor apps', 1, 5, 1, 0, GETDATE(), GETDATE(), 0),
    ('Blazor Hybrid Apps', 'Combining Web/Mobile UIs...', 'Cross-platform strategies', 1, 5, 2, 0, GETDATE(), GETDATE(), 0),
    ('Blazor JS Interop', 'JavaScript integration patterns...', 'Browser API access', 1, 5, 3, 0, GETDATE(), GETDATE(), 0),

    -- MAUI Articles (TopicId = 6)
    ('MAUI MVVM Pattern', 'Implementing ViewModel layers...', 'Architecture patterns', 1, 6, 1, 0, GETDATE(), GETDATE(), 0),
    ('MAUI Geolocation', 'Accessing device location...', 'Mobile hardware features', 1, 6, 2, 0, GETDATE(), GETDATE(), 0),
    ('MAUI Performance', 'Optimizing startup times...', 'Mobile app optimization', 1, 6, 3, 0, GETDATE(), GETDATE(), 0),

    -- SSR/CSR Articles (TopicId = 7)
    ('Next.js SSR Deep Dive', 'Implementing SSR with React...', 'SEO-friendly rendering', 1, 7, 1, 0, GETDATE(), GETDATE(), 0),
    ('CSR State Management', 'JWT handling in SPAs...', 'Client-side auth patterns', 1, 7, 2, 0, GETDATE(), GETDATE(), 0),
    ('Hydration Techniques', 'Blending SSR/CSR approaches...', 'Modern web architecture', 1, 7, 3, 0, GETDATE(), GETDATE(), 0);

-- Create Programming Tags
INSERT INTO Contents.Tag (TagName)
VALUES
    ('C#'), ('.NET'), ('Web Development'),
    ('Java'), ('Spring'), ('Concurrency'),
    ('React'), ('TypeScript'), ('Testing'),
    ('ORM'), ('SQL'), ('NoSQL'),
    ('Blazor'), ('WebAssembly'), ('Hybrid Apps'),
    ('MAUI'), ('Mobile'), ('MVVM'),
    ('SSR'), ('CSR'), ('Next.js'), ('Architecture');

-- Map Articles to Tags (example mappings)
INSERT INTO Contents.ArticleTag (ArticleId, TagId)
VALUES
    -- C# Articles
    (1,1), (1,2), (1,19),  -- C#, .NET, Architecture
    (2,1), (2,3), (2,10),  -- C#, Web Dev, ORM
    (3,1), (3,2), (3,14),  -- C#, .NET, Performance
   
    -- Java Articles
    (4,4), (4,5), (4,6),   -- Java, Spring, Concurrency
    (5,4), (5,5), (5,3),   -- Java, Spring, Web Dev
    (6,4), (6,6), (6,14),  -- Java, Concurrency, Performance
   
    -- React Articles
    (7,7), (7,19), (7,20), -- React, Architecture, Next.js
    (8,7), (8,8), (8,3),   -- React, TypeScript, Web Dev
    (9,7), (9,9), (9,14),  -- React, Testing, Performance
   
    -- EF Core Articles
    (10,10), (10,11), (10,14),  -- ORM, SQL, Performance
    (11,10), (11,11), (11,19),  -- ORM, SQL, Architecture
    (12,10), (12,12), (12,15),  -- ORM, NoSQL, Cloud
   
    -- Blazor Articles
    (13,13), (13,14), (13,3),   -- Blazor, WebAssembly, Web Dev
    (14,13), (14,15), (14,16),  -- Blazor, Hybrid Apps, Mobile
    (15,13), (15,3), (15,17),   -- Blazor, Web Dev, JS Interop
   
    -- MAUI Articles
    (16,16), (16,17), (16,18),  -- MAUI, Mobile, MVVM
    (17,16), (17,17), (17,19),  -- MAUI, Mobile, Architecture
    (18,16), (18,17), (18,14),  -- MAUI, Mobile, Performance
   
    -- SSR/CSR Articles
    (19,19), (19,20), (19,21),  -- SSR, CSR, Next.js
    (20,20), (20,3), (20,22),   -- CSR, Web Dev, Architecture
    (21,19), (21,20), (21,22);  -- SSR, CSR, Architecture