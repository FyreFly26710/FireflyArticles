using System;
using FF.Articles.Backend.Contents.API.Models.Entities;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public partial class ArticleRedisRepository
{
    private HashEntry[] GetHashEntries(Article entity)
    {
        var hash = new List<HashEntry>
            {
                new(ArticleProperty.Title, entity.Title),
                new(ArticleProperty.Content, entity.Content),
                new(ArticleProperty.Abstract, entity.Abstract),
                new(ArticleProperty.ArticleType, entity.ArticleType),
                new(ArticleProperty.ParentArticleId, entity.ParentArticleId?.ToString() ?? "0"),
                new(ArticleProperty.UserId, entity.UserId.ToString()),
                new(ArticleProperty.TopicId, entity.TopicId.ToString()),
                new(ArticleProperty.SortNumber, entity.SortNumber.ToString()),
                new(ArticleProperty.IsHidden, entity.IsHidden.ToString()),
                new(ArticleProperty.UpdateTime, entity.UpdateTime.Value.ToString("O")),
                new(ArticleProperty.CreateTime, entity.CreateTime.Value.ToString("O"))
            };
        return hash.ToArray();
    }
    private HashEntry GetHashEntry(Article entity, string propertyName)
    {
        RedisValue val = propertyName switch
        {
            ArticleProperty.Title => entity.Title,
            ArticleProperty.Content => entity.Content,
            ArticleProperty.Abstract => entity.Abstract,
            ArticleProperty.ArticleType => entity.ArticleType,
            ArticleProperty.ParentArticleId => entity.ParentArticleId?.ToString() ?? "0",
            ArticleProperty.UserId => entity.UserId.ToString(),
            ArticleProperty.TopicId => entity.TopicId.ToString(),
            ArticleProperty.SortNumber => entity.SortNumber.ToString(),
            ArticleProperty.IsHidden => entity.IsHidden.ToString(),
            ArticleProperty.UpdateTime => DateTime.UtcNow.ToString("O"),
            //ArticleProperty.CreateTime => entity.CreateTime.Value.ToString("O"),
            _ => throw new ArgumentException($"Invalid property name: {propertyName}")
        };
        return new HashEntry(propertyName, val);
    }
    private Article GetArticleFromHashEntry(HashEntry[] hash, int id)
    {
        var dict = hash.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

        var article = new Article
        {
            Id = id,
            Title = dict[ArticleProperty.Title],
            Content = dict[ArticleProperty.Content],
            Abstract = dict[ArticleProperty.Abstract],
            ArticleType = dict[ArticleProperty.ArticleType],
            ParentArticleId = dict.TryGetValue(ArticleProperty.ParentArticleId, out var parentId) && !string.IsNullOrEmpty(parentId)
                              ? int.Parse(parentId)
                              : null,
            UserId = int.Parse(dict[ArticleProperty.UserId]),
            TopicId = int.Parse(dict[ArticleProperty.TopicId]),
            SortNumber = int.Parse(dict[ArticleProperty.SortNumber]),
            IsHidden = int.Parse(dict[ArticleProperty.IsHidden]),
            CreateTime = DateTime.Parse(dict[ArticleProperty.CreateTime]),
            UpdateTime = DateTime.Parse(dict[ArticleProperty.UpdateTime])
        };
        return article;
    }
    private async Task UpdateIndexAsync(Article? oldEntity, Article? newEntity)
    {
        // Early return if both are null
        if (oldEntity == null && newEntity == null)
            return;

        var batch = _redis.CreateBatch();
        var tasks = new List<Task>();

        // Delete case: only oldEntity exists
        if (oldEntity != null && newEntity == null)
        {
            tasks.Add(batch.SetRemoveAsync($"{TOPIC_INDEX}{oldEntity.TopicId}", oldEntity.Id));
            if (oldEntity.ParentArticleId.HasValue)
            {
                tasks.Add(batch.SetRemoveAsync($"{PARENT_INDEX}{oldEntity.ParentArticleId}", oldEntity.Id));
            }
            tasks.Add(batch.SetRemoveAsync(ARTICLE_IDS_KEY, oldEntity.Id));
        }
        // Create case: only newEntity exists
        else if (newEntity != null && oldEntity == null)
        {
            tasks.Add(batch.SetAddAsync($"{TOPIC_INDEX}{newEntity.TopicId}", newEntity.Id));
            if (newEntity.ParentArticleId.HasValue)
            {
                tasks.Add(batch.SetAddAsync($"{PARENT_INDEX}{newEntity.ParentArticleId}", newEntity.Id));
            }
            tasks.Add(batch.SetAddAsync(ARTICLE_IDS_KEY, newEntity.Id));
        }
        // Update case: both entities exist
        else if (oldEntity != null && newEntity != null)
        {
            if (oldEntity.TopicId != newEntity.TopicId)
            {
                tasks.Add(batch.SetRemoveAsync($"{TOPIC_INDEX}{oldEntity.TopicId}", oldEntity.Id));
                tasks.Add(batch.SetAddAsync($"{TOPIC_INDEX}{newEntity.TopicId}", newEntity.Id));
            }
            if (oldEntity.ParentArticleId != newEntity.ParentArticleId)
            {
                if (oldEntity.ParentArticleId.HasValue)
                {
                    tasks.Add(batch.SetRemoveAsync($"{PARENT_INDEX}{oldEntity.ParentArticleId}", oldEntity.Id));
                }
                if (newEntity.ParentArticleId.HasValue)
                {
                    tasks.Add(batch.SetAddAsync($"{PARENT_INDEX}{newEntity.ParentArticleId}", newEntity.Id));
                }
            }
        }

        try
        {
            batch.Execute();
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update indexes for article {ArticleId}",
                newEntity?.Id ?? oldEntity?.Id);
            throw;
        }
    }
}
