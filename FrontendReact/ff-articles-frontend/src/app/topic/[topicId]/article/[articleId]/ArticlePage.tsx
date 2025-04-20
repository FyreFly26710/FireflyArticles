"use server";
import ArticleCard from "@/components/article/ArticleCard";
import ArticleSider from "@/components/article/ArticleSider";
import { fetchArticle, fetchTags, fetchTopic, fetchTopicArticle, fetchTopicList } from "@/app/topic/utils/fetcher";
import { Flex } from "antd";

const ArticlePage = async ({ params }: { params: { topicId: number, articleId: number } }) => {
    const { topicId, articleId } = params;
    const topic = await fetchTopic(topicId);
    if (!topic) return <div>Failed fetching topics, please refresh page.</div>;

    const [topicList, tagList, article] = await Promise.all([
        fetchTopicList(),
        fetchTags(),
        fetchArticle(articleId),
    ]);

    if (!article) return <div>Failed fetching article, please refresh page.</div>;

    return (
        <div id="articlePage" className="max-width-content">
            <Flex gap={12}>
                <ArticleSider topic={topic} parentArticleId={article.parentArticleId} articleId={article.articleId} />
                <ArticleCard article={article} topic={topic} topicList={topicList || []} tagList={tagList || []} />
            </Flex>
        </div>
    );
}

export default ArticlePage;