import { apiTopicGetById } from '@/api/contents/api/topic';
import { apiArticleGetById } from '@/api/contents/api/article';
import ArticleCard from '@/components/topic/ArticleCard';

// Set revalidation and dynamic rendering options
export const dynamic = 'force-dynamic'; // Options: 'auto' | 'force-dynamic' | 'error' | 'force-static'
// export const revalidate = 60; // Revalidate the data at most every 60 seconds

// Server component to fetch data
const TopicPage = async ({ params }: { params: { topicId: string } }) => {
    let topic: API.TopicDto | null = null;
    let article: API.ArticleDto | null = null;
    let error = null;
    const topicId = parseInt(params.topicId);

    try {
        const topicResponse = await apiTopicGetById({
            id: topicId,
        });
        const articleResponse = await apiArticleGetById({
            id: topicId,
            IncludeUser: true,
            IncludeContent: true
        });
        if (topicResponse.data) {
            topic = topicResponse.data;
        }
        if (articleResponse.data) {
            article = articleResponse.data;
        }
    } catch (err) {
        error = 'Failed to fetch topic data';
        console.error(error, err);
    }

    // Pass the server-fetched data to the client component
    return <ArticleCard topicId={topicId} article={article} error={error} />;
};

export default TopicPage;