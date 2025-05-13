namespace FF.Articles.Backend.Contents.API.Infrastructure;

public static class ContentsDbSeedData
{
    public static List<Tag> GetTags()
    {
        return new List<Tag>
        {
            new Tag { Id = 1, TagName = "Beginner", TagGroup="Skill Level"},
            new Tag { Id = 2, TagName = "Advanced", TagGroup="Skill Level"},
            new Tag { Id = 3, TagName = "Expert", TagGroup="Skill Level"},
            new Tag { Id = 4, TagName = "General", TagGroup="Skill Level"},
            new Tag { Id = 5, TagName = "Overview", TagGroup="Article Style"},
            new Tag { Id = 6, TagName = "Deep-dive", TagGroup="Article Style"},
            new Tag { Id = 7, TagName = "Best-practices", TagGroup="Article Style"},
            new Tag { Id = 8, TagName = "Listicle", TagGroup="Article Style"},
            new Tag { Id = 9, TagName = "Q&A", TagGroup="Article Style"},
            new Tag { Id = 10, TagName = "Comparison", TagGroup="Article Style"},
            new Tag { Id = 11, TagName = "Conversational", TagGroup="Tone"},
            new Tag { Id = 12, TagName = "Academic", TagGroup="Tone"},
            new Tag { Id = 13, TagName = "Technical", TagGroup="Tone"},
            new Tag { Id = 14, TagName = "Code-heavy", TagGroup="Tone"},
            new Tag { Id = 15, TagName = "Performance Optimization", TagGroup="Focus Area"},
            new Tag { Id = 16, TagName = "C#", TagGroup="Tech Stack/Language"},
        };
    }


    public static List<Topic> GetTopics()
    {
        return new List<Topic>
        {
            new Topic { Id = 100, Title = "Java Fundamentals", Abstract = "Introduction to Java", Category = "Java", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 200, Title = "Advanced Java", Abstract = "Advanced Java topics", Category = "Java", UserId = 1, SortNumber = 2, IsHidden = 0 },
            new Topic { Id = 300, Title = "C# Basics", Abstract = "C# fundamentals", Category = "C#", UserId = 1, SortNumber = 3, IsHidden = 0 },
            new Topic { Id = 400, Title = "C# Advanced", Abstract = "Advanced C# topics", Category = "C#", UserId = 1, SortNumber = 4, IsHidden = 0 },
            new Topic { Id = 500, Title = "Cross-Platform Development", Abstract = "Cross-platform development", Category = "C#", UserId = 1, SortNumber = 5, IsHidden = 0 },
            new Topic { Id = 600, Title = "Test1", Abstract = "Test1", Category = "TestCategory1", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 700, Title = "Test2", Abstract = "Test2", Category = "TestCategory2", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 800, Title = "Test3", Abstract = "Test3", Category = "TestCategory3", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 900, Title = "Test4", Abstract = "Test4", Category = "TestCategory4", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 1000, Title = "Test5", Abstract = "Test5", Category = "TestCategory5", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 1100, Title = "Test6", Abstract = "Test6", Category = "TestCategory6", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 1200, Title = "Test7", Abstract = "Test7", Category = "TestCategory7", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 1300, Title = "Test8", Abstract = "Test8", Category = "TestCategory8", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 1400, Title = "Test9", Abstract = "Test9", Category = "TestCategory9", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 1500, Title = "Test10", Abstract = "Test10", Category = "TestCategory10", UserId = 1, SortNumber = 1, IsHidden = 0 }
        };
    }
    public static List<Article> GetTopicArticles()
    {
        return new List<Article>
        {
            new Article { Id = 100, Title = "Java Fundamentals", Content = "Learn the basics of Java programming", Abstract = "Introduction to Java", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 200, Title = "Advanced Java", Content = "Advanced Java concepts and patterns", Abstract = "Advanced Java topics", UserId = 1, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 300, Title = "C# Basics", Content = "Introduction to C# programming", Abstract = "C# fundamentals", UserId = 1, SortNumber = 3, IsHidden = 0 },
            new Article { Id = 400, Title = "C# Advanced", Content = "Advanced C# concepts and features", Abstract = "Advanced C# topics", UserId = 1, SortNumber = 4, IsHidden = 0 },
            new Article { Id = 500, Title = "Cross-Platform Development", Content = "Developing cross-platform applications", Abstract = "Cross-platform development", UserId = 1, SortNumber = 5, IsHidden = 0 }
        };
    }

    public static List<Article> GetArticles()
    {
        var articles = new List<Article>
        {
            // Java Fundamentals Topic Articles
            new Article { Id = 1, Title = "Introduction to Java", Content = "Java basics content", Abstract = "Java basics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 100, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 2, Title = "Java Variables and Data Types", Content = "Variables content", Abstract = "Variables", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 1, UserId = 1, TopicId = 100, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 3, Title = "Java Control Structures", Content = "Control structures content", Abstract = "Control structures", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 100, SortNumber = 3, IsHidden = 0 },
            new Article { Id = 16, Title = "Java Object-Oriented Programming", Content = "OOP concepts in Java", Abstract = "Java OOP", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 100, SortNumber = 4, IsHidden = 0 },
            new Article { Id = 17, Title = "Java Inheritance and Polymorphism", Content = "Inheritance and polymorphism content", Abstract = "Java inheritance", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 16, UserId = 11, TopicId = 100, SortNumber = 5, IsHidden = 0 },
            new Article { Id = 26, Title = "Java Exception Handling", Content = "Exception handling in Java", Abstract = "Java exceptions", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 100, SortNumber = 6, IsHidden = 0 },
            new Article { Id = 27, Title = "Java File I/O", Content = "File I/O operations in Java", Abstract = "Java I/O", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 26, UserId = 1, TopicId = 100, SortNumber = 7, IsHidden = 0 },
            new Article { Id = 28, Title = "Java Networking", Content = "Networking in Java", Abstract = "Java networking", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 100, SortNumber = 8, IsHidden = 0 },

            // Advanced Java Topic Articles
            new Article { Id = 4, Title = "Java Design Patterns", Content = "Design patterns content", Abstract = "Design patterns", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 200, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 5, Title = "Java Multithreading", Content = "Multithreading content", Abstract = "Multithreading", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 200, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 6, Title = "Java Collections Framework", Content = "Collections content", Abstract = "Collections", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 4, UserId = 1, TopicId = 200, SortNumber = 3, IsHidden = 0 },
            new Article { Id = 18, Title = "Java Stream API", Content = "Stream API content", Abstract = "Java streams", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 200, SortNumber = 4, IsHidden = 0 },
            new Article { Id = 19, Title = "Java Lambda Expressions", Content = "Lambda expressions content", Abstract = "Java lambdas", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 18, UserId = 11, TopicId = 200, SortNumber = 5, IsHidden = 0 },
            new Article { Id = 29, Title = "Java Concurrency", Content = "Concurrency in Java", Abstract = "Java concurrency", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 200, SortNumber = 6, IsHidden = 0 },
            new Article { Id = 30, Title = "Java Memory Management", Content = "Memory management in Java", Abstract = "Java memory", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 29, UserId = 1, TopicId = 200, SortNumber = 7, IsHidden = 0 },
            new Article { Id = 31, Title = "Java Performance Tuning", Content = "Performance tuning in Java", Abstract = "Java performance", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 200, SortNumber = 8, IsHidden = 0 },

            // C# Basics Topic Articles
            new Article { Id = 7, Title = "Introduction to C#", Content = "C# basics content", Abstract = "C# basics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 300, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 8, Title = "C# Variables and Types", Content = "Variables content", Abstract = "Variables", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 7, UserId = 10, TopicId = 300, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 9, Title = "C# Control Flow", Content = "Control flow content", Abstract = "Control flow", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 300, SortNumber = 3, IsHidden = 0 },
            new Article { Id = 20, Title = "C# Object-Oriented Programming", Content = "OOP in C#", Abstract = "C# OOP", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 300, SortNumber = 4, IsHidden = 0 },
            new Article { Id = 21, Title = "C# Interfaces and Abstract Classes", Content = "Interfaces and abstract classes content", Abstract = "C# interfaces", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 20, UserId = 1, TopicId = 300, SortNumber = 5, IsHidden = 0 },
            new Article { Id = 32, Title = "C# Exception Handling", Content = "Exception handling in C#", Abstract = "C# exceptions", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 300, SortNumber = 6, IsHidden = 0 },
            new Article { Id = 33, Title = "C# File Operations", Content = "File operations in C#", Abstract = "C# files", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 32, UserId = 10, TopicId = 300, SortNumber = 7, IsHidden = 0 },
            new Article { Id = 34, Title = "C# Networking", Content = "Networking in C#", Abstract = "C# networking", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 300, SortNumber = 8, IsHidden = 0 },

            // C# Advanced Topic Articles
            new Article { Id = 10, Title = "C# Advanced Features", Content = "Advanced features content", Abstract = "Advanced features", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 400, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 11, Title = "C# LINQ", Content = "LINQ content", Abstract = "LINQ", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 400, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 12, Title = "C# Async/Await", Content = "Async/Await content", Abstract = "Async/Await", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 10, UserId = 10, TopicId = 400, SortNumber = 3, IsHidden = 0 },
            new Article { Id = 22, Title = "C# Generics", Content = "Generics content", Abstract = "C# generics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 400, SortNumber = 4, IsHidden = 0 },
            new Article { Id = 23, Title = "C# Reflection", Content = "Reflection content", Abstract = "C# reflection", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 22, UserId = 1, TopicId = 400, SortNumber = 5, IsHidden = 0 },
            new Article { Id = 35, Title = "C# Concurrency", Content = "Concurrency in C#", Abstract = "C# concurrency", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 400, SortNumber = 6, IsHidden = 0 },
            new Article { Id = 36, Title = "C# Memory Management", Content = "Memory management in C#", Abstract = "C# memory", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 35, UserId = 10, TopicId = 400, SortNumber = 7, IsHidden = 0 },
            new Article { Id = 37, Title = "C# Performance Optimization", Content = "Performance optimization in C#", Abstract = "C# performance", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 400, SortNumber = 8, IsHidden = 0 },

            // Cross-Platform Development Topic Articles
            new Article { Id = 13, Title = "Cross-Platform Basics", Content = "Cross-platform basics content", Abstract = "Cross-platform basics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 500, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 14, Title = "Xamarin Development", Content = "Xamarin content", Abstract = "Xamarin", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 500, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 15, Title = ".NET MAUI", Content = "MAUI content", Abstract = "MAUI", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 13, UserId = 11, TopicId = 500, SortNumber = 3, IsHidden = 0 },
            new Article { Id = 24, Title = "Blazor Web Development", Content = "Blazor content", Abstract = "Blazor", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 500, SortNumber = 4, IsHidden = 0 },
            new Article { Id = 25, Title = "Blazor Components", Content = "Blazor components content", Abstract = "Blazor components", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 24, UserId = 1, TopicId = 500, SortNumber = 5, IsHidden = 0 },
            new Article { Id = 38, Title = "Cross-Platform UI Design", Content = "UI design for cross-platform apps", Abstract = "Cross-platform UI", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 10, TopicId = 500, SortNumber = 6, IsHidden = 0 },
            new Article { Id = 39, Title = "Platform-Specific Features", Content = "Handling platform-specific features", Abstract = "Platform features", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 38, UserId = 10, TopicId = 500, SortNumber = 7, IsHidden = 0 },
            new Article { Id = 40, Title = "Cross-Platform Testing", Content = "Testing cross-platform applications", Abstract = "Cross-platform testing", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 11, TopicId = 500, SortNumber = 8, IsHidden = 0 }
        };
        return articles;
    }

    public static List<ArticleTag> GetArticleTags()
    {
        return new List<ArticleTag>
        {
            // Java Fundamentals Topic Articles
            new ArticleTag { Id = 1, ArticleId = 1, TagId = 1 },
            new ArticleTag { Id = 2, ArticleId = 1, TagId = 9 },
            new ArticleTag { Id = 3, ArticleId = 2, TagId = 1 },
            new ArticleTag { Id = 4, ArticleId = 3, TagId = 1 },
            new ArticleTag { Id = 5, ArticleId = 3, TagId = 4 },
            new ArticleTag { Id = 26, ArticleId = 16, TagId = 1 },
            new ArticleTag { Id = 27, ArticleId = 16, TagId = 4 },
            new ArticleTag { Id = 28, ArticleId = 17, TagId = 1 },
            new ArticleTag { Id = 29, ArticleId = 17, TagId = 9 },
            new ArticleTag { Id = 46, ArticleId = 26, TagId = 1 },
            new ArticleTag { Id = 47, ArticleId = 26, TagId = 9 },
            new ArticleTag { Id = 48, ArticleId = 27, TagId = 1 },
            new ArticleTag { Id = 49, ArticleId = 27, TagId = 6 },
            new ArticleTag { Id = 50, ArticleId = 28, TagId = 1 },
            new ArticleTag { Id = 51, ArticleId = 28, TagId = 7 },

            // Advanced Java Topic Articles
            new ArticleTag { Id = 6, ArticleId = 4, TagId = 4 },
            new ArticleTag { Id = 7, ArticleId = 4, TagId = 8 },
            new ArticleTag { Id = 8, ArticleId = 5, TagId = 6 },
            new ArticleTag { Id = 9, ArticleId = 6, TagId = 4 },
            new ArticleTag { Id = 10, ArticleId = 6, TagId = 9 },
            new ArticleTag { Id = 30, ArticleId = 18, TagId = 1 },
            new ArticleTag { Id = 31, ArticleId = 18, TagId = 6 },
            new ArticleTag { Id = 32, ArticleId = 19, TagId = 1 },
            new ArticleTag { Id = 33, ArticleId = 19, TagId = 9 },
            new ArticleTag { Id = 52, ArticleId = 29, TagId = 1 },
            new ArticleTag { Id = 53, ArticleId = 29, TagId = 6 },
            new ArticleTag { Id = 54, ArticleId = 30, TagId = 1 },
            new ArticleTag { Id = 55, ArticleId = 30, TagId = 8 },
            new ArticleTag { Id = 56, ArticleId = 31, TagId = 1 },
            new ArticleTag { Id = 57, ArticleId = 31, TagId = 6 },

            // C# Basics Topic Articles
            new ArticleTag { Id = 11, ArticleId = 7, TagId = 1 },
            new ArticleTag { Id = 12, ArticleId = 7, TagId = 9 },
            new ArticleTag { Id = 13, ArticleId = 8, TagId = 1 },
            new ArticleTag { Id = 14, ArticleId = 9, TagId = 1 },
            new ArticleTag { Id = 15, ArticleId = 9, TagId = 4 },
            new ArticleTag { Id = 34, ArticleId = 20, TagId = 1 },
            new ArticleTag { Id = 35, ArticleId = 20, TagId = 4 },
            new ArticleTag { Id = 36, ArticleId = 21, TagId = 1 },
            new ArticleTag { Id = 37, ArticleId = 21, TagId = 9 },
            new ArticleTag { Id = 58, ArticleId = 32, TagId = 1 },
            new ArticleTag { Id = 59, ArticleId = 32, TagId = 9 },
            new ArticleTag { Id = 60, ArticleId = 33, TagId = 1 },
            new ArticleTag { Id = 61, ArticleId = 33, TagId = 6 },
            new ArticleTag { Id = 62, ArticleId = 34, TagId = 1 },
            new ArticleTag { Id = 63, ArticleId = 34, TagId = 7 },

            // C# Advanced Topic Articles
            new ArticleTag { Id = 16, ArticleId = 10, TagId = 4 },
            new ArticleTag { Id = 17, ArticleId = 10, TagId = 8 },
            new ArticleTag { Id = 18, ArticleId = 11, TagId = 6 },
            new ArticleTag { Id = 19, ArticleId = 12, TagId = 4 },
            new ArticleTag { Id = 20, ArticleId = 12, TagId = 9 },
            new ArticleTag { Id = 38, ArticleId = 22, TagId = 1 },
            new ArticleTag { Id = 39, ArticleId = 22, TagId = 4 },
            new ArticleTag { Id = 40, ArticleId = 23, TagId = 1 },
            new ArticleTag { Id = 41, ArticleId = 23, TagId = 8 },
            new ArticleTag { Id = 64, ArticleId = 35, TagId = 1 },
            new ArticleTag { Id = 65, ArticleId = 35, TagId = 6 },
            new ArticleTag { Id = 66, ArticleId = 36, TagId = 1 },
            new ArticleTag { Id = 67, ArticleId = 36, TagId = 8 },
            new ArticleTag { Id = 68, ArticleId = 37, TagId = 1 },
            new ArticleTag { Id = 69, ArticleId = 37, TagId = 6 },

            // Cross-Platform Development Topic Articles
            new ArticleTag { Id = 21, ArticleId = 13, TagId = 2 },
            new ArticleTag { Id = 22, ArticleId = 13, TagId = 8 },
            new ArticleTag { Id = 23, ArticleId = 14, TagId = 2 },
            new ArticleTag { Id = 24, ArticleId = 15, TagId = 2 },
            new ArticleTag { Id = 25, ArticleId = 15, TagId = 8 },
            new ArticleTag { Id = 42, ArticleId = 24, TagId = 2 },
            new ArticleTag { Id = 43, ArticleId = 24, TagId = 8 },
            new ArticleTag { Id = 44, ArticleId = 25, TagId = 2 },
            new ArticleTag { Id = 45, ArticleId = 25, TagId = 9 },
            new ArticleTag { Id = 70, ArticleId = 38, TagId = 2 },
            new ArticleTag { Id = 71, ArticleId = 38, TagId = 8 },
            new ArticleTag { Id = 72, ArticleId = 39, TagId = 2 },
            new ArticleTag { Id = 73, ArticleId = 39, TagId = 9 },
            new ArticleTag { Id = 74, ArticleId = 40, TagId = 2 },
            new ArticleTag { Id = 75, ArticleId = 40, TagId = 5 }
        };
    }
}
