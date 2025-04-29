"use server";
import { apiTopicGetById, apiTopicGetByPage } from "@/api/contents/api/topic";
import { apiArticleGetById } from "@/api/contents/api/article";
import { apiTagGetAll } from "@/api/contents/api/tag";

// Fetch topic by ID
export async function fetchTopic(topicId: number): Promise<API.TopicDto | undefined> {
    try {
        const topicRes = await apiTopicGetById({ id: topicId, IncludeArticles: true, IncludeSubArticles: true });
        return topicRes.data;
    } catch (e: any) {
        console.error("Failed fetching topic:", e.message);
        return undefined;
    }
}

// Fetch paginated topic list
export async function fetchTopicList(): Promise<API.TopicDto[] | undefined> {
    try {
        const topicListRes = await apiTopicGetByPage({
            PageNumber: 1,
            PageSize: 100,
            IncludeArticles: true,
        });
        return topicListRes.data?.data ?? [];
    } catch (e: any) {
        console.error("Failed fetching topics:", e.message);
        return undefined;
    }
}

// Fetch all tags
export async function fetchTags(): Promise<API.TagDto[] | undefined> {
    try {
        const tagRes = await apiTagGetAll();
        return tagRes.data;
    } catch (e: any) {
        console.error("Failed fetching tags:", e.message);
        return undefined;
    }
}

// Fetch an article by ID
export async function fetchArticle(articleId: number): Promise<API.ArticleDto | undefined> {
    try {
        // if (isNewArticle) {
        //     return { articleType: "Article", topicId: topicId, title: topic?.title } as API.ArticleDto;
        // } else 
        const articleRes = await apiArticleGetById({ id: articleId, IncludeContent: true });
        return articleRes.data;

    } catch (e: any) {
        console.error("Failed fetching article:", e.message);
        return undefined;
    }
}

// Fetch a topic article by ID
export async function fetchTopicArticle(topic: API.TopicDto)
    : Promise<API.ArticleDto | undefined> {
    try {
        return {
            articleType: "TopicArticle",
            topicId: topic.topicId,
            title: topic.title,
            abstract: topic.abstract,
            content: topic.content,
        } as API.ArticleDto;
    } catch (e: any) {
        console.error("Failed fetching topic article:", e.message);
        return undefined;
    }
}