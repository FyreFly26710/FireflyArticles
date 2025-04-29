"use client";
import ArticleCard from "@/components/article/ArticleCard";
import ArticleSider from "@/components/article/ArticleSider";
import { Flex, Spin } from "antd";
import { useArticle, useTags, useTopic, useTopicList } from "@/app/topic/hooks";

const ArticlePage = ({ params }: { params: { topicId: number, articleId: number } }) => {
    const { topicId, articleId } = params;

    const { topic, loading: topicLoading, error: topicError } = useTopic(topicId);
    const { article, loading: articleLoading, error: articleError } = useArticle(articleId);
    const { topicList, loading: topicListLoading } = useTopicList();
    const { tagList, loading: tagListLoading } = useTags();

    const isLoading = topicLoading || articleLoading || topicListLoading || tagListLoading;

    if (isLoading) {
        return (
            <div className="flex justify-center items-center h-full">
                <Spin size="large" />
            </div>
        );
    }

    if (topicError || !topic) {
        return <div>Failed fetching topic, please refresh page.</div>;
    }

    if (articleError || !article) {
        return <div>Failed fetching article, please refresh page.</div>;
    }

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