import { ReactNode } from 'react';
import { apiTopicGetById } from '@/api/contents/api/topic';
import ArticleLayout from '@/layouts/ArticleLayout';

interface TopicLayoutProps {
    children: ReactNode;
    params: { topicId: string };
}

export default async function TopicLayout({ children, params }: TopicLayoutProps) {
    let topic: API.TopicDto | null = null;

    try {
        const topicId = parseInt(params.topicId);

        if (!isNaN(topicId)) {
            const response = await apiTopicGetById({
                id: topicId,
                IncludeUser: true,
                IncludeArticles: true,
                IncludeSubArticles: true
            });

            if (response.data) {
                topic = response.data;
            }
        }
    } catch (err) {
        console.error('Failed to fetch topic data', err);
    }

    return <ArticleLayout topic={topic}>{children}</ArticleLayout>;
} 