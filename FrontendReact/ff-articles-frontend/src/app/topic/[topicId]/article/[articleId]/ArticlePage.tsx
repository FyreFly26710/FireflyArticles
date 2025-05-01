
import { apiArticleGetById } from '@/api/contents/api/article';
import ArticleCard from '@/components/topic/ArticleCard';

export const dynamic = 'force-dynamic';

// Server component to fetch data
const ArticlePage = async ({ params }: { params: { topicId: string, articleId: string } }) => {
    var article: API.ArticleDto;

    try {
        const response = await apiArticleGetById({
            id: parseInt(params.articleId),
            IncludeUser: true,
            IncludeContent: true
        });

        if (!response.data) {
            throw new Error('Empty article data');
        }

        article = response.data;

    } catch (err) {
        var error = 'Failed to fetch article data';
        console.error(error, err);
        return <div>Error: {error}</div>;

    }

    // Pass the server-fetched data to the client component
    return <ArticleCard article={article} topicId={parseInt(params.topicId)} />;
};

export default ArticlePage;
