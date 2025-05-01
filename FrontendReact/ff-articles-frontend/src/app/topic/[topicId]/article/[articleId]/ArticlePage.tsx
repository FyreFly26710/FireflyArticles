
import { apiArticleGetById } from '@/api/contents/api/article';
import ArticleCard from '@/components/topic/ArticleCard';

export const dynamic = 'force-dynamic';

// Server component to fetch data
const ArticlePage = async ({ params }: { params: { topicId: string, articleId: string } }) => {
    let article: API.ArticleDto | null = null;
    let error = null;

    try {
        const response = await apiArticleGetById({
            id: parseInt(params.articleId),
            IncludeUser: true,
            IncludeContent: true
        });

        if (response.data) {
            article = response.data;
        }
    } catch (err) {
        error = 'Failed to fetch article data';
        console.error(error, err);
    }

    // Pass the server-fetched data to the client component
    return <ArticleCard article={article} error={error} topicId={parseInt(params.topicId)} />;
};

export default ArticlePage;
