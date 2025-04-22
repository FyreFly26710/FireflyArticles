import { useState, useEffect } from 'react';
import { apiTopicGetByPage } from '@/api/contents/api/topic';
import { apiArticleGetByPage } from '@/api/contents/api/article';

export function useHomePageData() {
    const [topicList, setTopicList] = useState<API.TopicDto[]>([]);
    const [articleList, setArticleList] = useState<API.ArticleDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                setLoading(true);

                // Fetch topics and articles in parallel
                const [topicsResponse, articlesResponse] = await Promise.all([
                    apiTopicGetByPage({
                        PageSize: 12,
                        IncludeContent: false,
                        IncludeArticles: false,
                        IncludeSubArticles: false,
                        IncludeUser: false,
                    }),
                    apiArticleGetByPage({
                        PageSize: 10,
                        IncludeContent: false,
                        IncludeSubArticles: false,
                        IncludeUser: false,
                        DisplaySubArticles: false,
                    })
                ]);

                setTopicList(topicsResponse.data?.data ?? []);
                setArticleList(articlesResponse.data?.data ?? []);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to fetch homepage data'));
                console.error("Failed fetching homepage data: ", err);
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, []);

    return { topicList, articleList, loading, error };
} 