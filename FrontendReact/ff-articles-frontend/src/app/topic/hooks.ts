import { useState, useEffect } from 'react';
import { apiTopicGetById, apiTopicGetByPage } from '@/api/contents/api/topic';
import { apiArticleGetById } from '@/api/contents/api/article';
import { apiTagGetAll } from '@/api/contents/api/tag';

export function useTopic(topicId: number) {
    const [topic, setTopic] = useState<API.TopicDto | undefined>(undefined);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                setLoading(true);
                const response = await apiTopicGetById({
                    id: topicId,
                    IncludeArticles: true
                });
                setTopic(response.data);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to fetch topic'));
            } finally {
                setLoading(false);
            }
        }

        if (topicId) {
            fetchData();
        }
    }, [topicId]);

    return { topic, loading, error };
}

export function useTopicList() {
    const [topicList, setTopicList] = useState<API.TopicDto[] | undefined>(undefined);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                setLoading(true);
                const response = await apiTopicGetByPage({
                    PageSize: 100,
                    IncludeArticles: false
                });
                setTopicList(response.data?.data);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to fetch topic list'));
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, []);

    return { topicList, loading, error };
}

export function useArticle(articleId: number) {
    const [article, setArticle] = useState<API.ArticleDto | undefined>(undefined);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                setLoading(true);
                const response = await apiArticleGetById({
                    id: articleId,
                    IncludeContent: true,
                    IncludeUser: true,
                    IncludeSubArticles: true
                });
                setArticle(response.data);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to fetch article'));
            } finally {
                setLoading(false);
            }
        }

        if (articleId) {
            fetchData();
        }
    }, [articleId]);

    return { article, loading, error };
}

export function useTags() {
    const [tagList, setTagList] = useState<API.TagDto[] | undefined>(undefined);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                setLoading(true);
                const response = await apiTagGetAll();
                setTagList(response.data);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to fetch tags'));
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, []);

    return { tagList, loading, error };
} 