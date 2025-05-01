using System;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Infrastructure;

public static class ContentsDbSeedData
{
    public static List<Tag> GetTags()
    {
        return new List<Tag>
        {
            new Tag { Id = 1, TagName = "Programming" },
            new Tag { Id = 2, TagName = "Web Development" },
            new Tag { Id = 3, TagName = "Database" },
            new Tag { Id = 4, TagName = "Design Patterns" },
            new Tag { Id = 5, TagName = "Testing" },
            new Tag { Id = 6, TagName = "Performance" },
            new Tag { Id = 7, TagName = "Security" },
            new Tag { Id = 8, TagName = "Architecture" },
            new Tag { Id = 9, TagName = "Best Practices" },
            new Tag { Id = 10, TagName = "Tutorial" }
        };
    }

    public static List<Topic> GetTopics()
    {
        return new List<Topic>
        {
            new Topic { Id = 100, Title = "Java Fundamentals", Content = "Learn the basics of Java programming", Abstract = "Introduction to Java", Category = "Java", UserId = 1, SortNumber = 1, IsHidden = 0 },
            new Topic { Id = 200, Title = "Advanced Java", Content = "Advanced Java concepts and patterns", Abstract = "Advanced Java topics", Category = "Java", UserId = 1, SortNumber = 2, IsHidden = 0 },
            new Topic { Id = 300, Title = "C# Basics", Content = "Introduction to C# programming", Abstract = "C# fundamentals", Category = "C#", UserId = 1, SortNumber = 3, IsHidden = 0 },
            new Topic { Id = 400, Title = "C# Advanced", Content = "Advanced C# concepts and features", Abstract = "Advanced C# topics", Category = "C#", UserId = 1, SortNumber = 4, IsHidden = 0 },
            new Topic { Id = 500, Title = "Cross-Platform Development", Content = "Developing cross-platform applications", Abstract = "Cross-platform development", Category = "C#", UserId = 1, SortNumber = 5, IsHidden = 0 }
        };
    }
    //public static List<Article> GetTopicArticles()
    //{
    //    return new List<Article>
    //    {
    //        new Article { Id = 100, Title = "Java Fundamentals", Content = "Learn the basics of Java programming", Abstract = "Introduction to Java", UserId = 1, SortNumber = 1, IsHidden = 0 },
    //        new Article { Id = 200, Title = "Advanced Java", Content = "Advanced Java concepts and patterns", Abstract = "Advanced Java topics", UserId = 1, SortNumber = 2, IsHidden = 0 },
    //        new Article { Id = 300, Title = "C# Basics", Content = "Introduction to C# programming", Abstract = "C# fundamentals", UserId = 1, SortNumber = 3, IsHidden = 0 },
    //        new Article { Id = 400, Title = "C# Advanced", Content = "Advanced C# concepts and features", Abstract = "Advanced C# topics", UserId = 1, SortNumber = 4, IsHidden = 0 },
    //        new Article { Id = 500, Title = "Cross-Platform Development", Content = "Developing cross-platform applications", Abstract = "Cross-platform development", UserId = 1, SortNumber = 5, IsHidden = 0 }
    //    };
    //}

    public static List<Article> GetArticles()
    {
        return new List<Article>
        {
            // Java Fundamentals Topic Articles
            new Article { Id = 1, Title = "Introduction to Java", Content = "Java basics content", Abstract = "Java basics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 1, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 2, Title = "Java Variables and Data Types", Content = "Variables content", Abstract = "Variables", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 1, UserId = 1, TopicId = 1, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 3, Title = "Java Control Structures", Content = "Control structures content", Abstract = "Control structures", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 1, SortNumber = 3, IsHidden = 0 },

            // Advanced Java Topic Articles
            new Article { Id = 4, Title = "Java Design Patterns", Content = "Design patterns content", Abstract = "Design patterns", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 2, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 5, Title = "Java Multithreading", Content = "Multithreading content", Abstract = "Multithreading", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 2, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 6, Title = "Java Collections Framework", Content = "Collections content", Abstract = "Collections", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 4, UserId = 1, TopicId = 2, SortNumber = 3, IsHidden = 0 },

            // C# Basics Topic Articles
            new Article { Id = 7, Title = "Introduction to C#", Content = "C# basics content", Abstract = "C# basics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 3, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 8, Title = "C# Variables and Types", Content = "Variables content", Abstract = "Variables", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 7, UserId = 1, TopicId = 3, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 9, Title = "C# Control Flow", Content = "Control flow content", Abstract = "Control flow", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 3, SortNumber = 3, IsHidden = 0 },

            // C# Advanced Topic Articles
            new Article { Id = 10, Title = "C# Advanced Features", Content = "Advanced features content", Abstract = "Advanced features", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 4, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 11, Title = "C# LINQ", Content = "LINQ content", Abstract = "LINQ", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 4, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 12, Title = "C# Async/Await", Content = "Async/Await content", Abstract = "Async/Await", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 10, UserId = 1, TopicId = 4, SortNumber = 3, IsHidden = 0 },

            // Cross-Platform Development Topic Articles
            new Article { Id = 13, Title = "Cross-Platform Basics", Content = "Cross-platform basics content", Abstract = "Cross-platform basics", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 5, SortNumber = 1, IsHidden = 0 },
            new Article { Id = 14, Title = "Xamarin Development", Content = "Xamarin content", Abstract = "Xamarin", ArticleType = ArticleTypes.Article, ParentArticleId = null, UserId = 1, TopicId = 5, SortNumber = 2, IsHidden = 0 },
            new Article { Id = 15, Title = ".NET MAUI", Content = "MAUI content", Abstract = "MAUI", ArticleType = ArticleTypes.SubArticle, ParentArticleId = 13, UserId = 1, TopicId = 5, SortNumber = 3, IsHidden = 0 }
        };
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

            // Advanced Java Topic Articles
            new ArticleTag { Id = 6, ArticleId = 4, TagId = 4 },
            new ArticleTag { Id = 7, ArticleId = 4, TagId = 8 },
            new ArticleTag { Id = 8, ArticleId = 5, TagId = 6 },
            new ArticleTag { Id = 9, ArticleId = 6, TagId = 4 },
            new ArticleTag { Id = 10, ArticleId = 6, TagId = 9 },

            // C# Basics Topic Articles
            new ArticleTag { Id = 11, ArticleId = 7, TagId = 1 },
            new ArticleTag { Id = 12, ArticleId = 7, TagId = 9 },
            new ArticleTag { Id = 13, ArticleId = 8, TagId = 1 },
            new ArticleTag { Id = 14, ArticleId = 9, TagId = 1 },
            new ArticleTag { Id = 15, ArticleId = 9, TagId = 4 },

            // C# Advanced Topic Articles
            new ArticleTag { Id = 16, ArticleId = 10, TagId = 4 },
            new ArticleTag { Id = 17, ArticleId = 10, TagId = 8 },
            new ArticleTag { Id = 18, ArticleId = 11, TagId = 6 },
            new ArticleTag { Id = 19, ArticleId = 12, TagId = 4 },
            new ArticleTag { Id = 20, ArticleId = 12, TagId = 9 },

            // Cross-Platform Development Topic Articles
            new ArticleTag { Id = 21, ArticleId = 13, TagId = 2 },
            new ArticleTag { Id = 22, ArticleId = 13, TagId = 8 },
            new ArticleTag { Id = 23, ArticleId = 14, TagId = 2 },
            new ArticleTag { Id = 24, ArticleId = 15, TagId = 2 },
            new ArticleTag { Id = 25, ArticleId = 15, TagId = 8 }
        };
    }
}
