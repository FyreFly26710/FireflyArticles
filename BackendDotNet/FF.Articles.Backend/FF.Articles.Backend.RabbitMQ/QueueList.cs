using System;

namespace FF.Articles.Backend.RabbitMQ;

public static class QueueList
{
    public const string GenerateArticleQueue = "generate_article_queue";
    public const string ArticleReadyQueue = "article_ready_queue";
    public const string AddArticleQueue = "add_article_queue";
}
// public enum QueueList
// {
//     GenerateArticleQueue,
//     ArticleReadyQueue
// }

