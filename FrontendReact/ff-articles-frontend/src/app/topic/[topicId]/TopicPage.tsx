import { apiTopicGetById } from '@/api/contents/api/topic';
import { apiArticleGetById } from '@/api/contents/api/article';
import ArticleCard from '@/components/article/ArticleCard';

// Set revalidation and dynamic rendering options
export const dynamic = 'force-dynamic'; // Options: 'auto' | 'force-dynamic' | 'error' | 'force-static'
// export const revalidate = 60; // Revalidate the data at most every 60 seconds

// Server component to fetch data
const TopicPage = async ({ params }: { params: { topicId: string } }) => {
    var article: API.ArticleDto;
    const topicId = parseInt(params.topicId);

    try {
        const topicResponse = await apiTopicGetById({
            id: topicId,
        });
        var topic = topicResponse.data;

        if (!topic) {
            throw new Error('Empty topic data');
        }

        const articleResponse = await apiArticleGetById({
            id: topicId,
            IncludeUser: true,
            IncludeContent: true
        });
        article = articleResponse.data ?? {
            articleId: topicId,
            title: topic?.title ?? '',
            content: ''
        };
    } catch (err) {
        var error = 'Failed to fetch topic data';
        console.error(error, err);
        return <div>Error: {error}</div>;

    }

    return <ArticleCard topicId={topicId} article={article} />;
};

export default TopicPage;