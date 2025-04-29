import { useState, useEffect } from 'react';
import { apiTopicGetByPage } from '@/api/contents/api/topic';

export function useTopics(pageSize: number = 200) {
    const [topicList, setTopicList] = useState<API.TopicDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                setLoading(true);
                const res = await apiTopicGetByPage({
                    PageSize: pageSize,
                });
                setTopicList(res.data?.data ?? []);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to fetch topics'));
                console.error("Failed fetching topics: ", err);
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, [pageSize]);

    return { topicList, loading, error };
} 