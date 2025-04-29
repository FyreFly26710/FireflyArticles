"use client";
import ArticleCard from "@/components/article/ArticleCard";
import ArticleSider from "@/components/article/ArticleSider";
import { useTags, useTopic, useTopicList } from "@/app/topic/hooks";
import { Flex, Spin } from "antd";

const TopicPage = ({ params }: { params: { topicId: number } }) => {
    const { topicId } = params;

    const { topic, loading: topicLoading, error: topicError } = useTopic(topicId);
    const { topicList, loading: topicListLoading } = useTopicList();
    const { tagList, loading: tagListLoading } = useTags();

    const isLoading = topicLoading || topicListLoading || tagListLoading;

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

    const article = {
        articleType: "TopicArticle",
        topicId: topic.topicId,
        title: topic.title,
        abstract: topic.abstract,
        content: topic.content,
    } as API.ArticleDto;

    return (
        <div id="articlePage" className="max-width-content">
            <Flex gap={12}>
                <ArticleSider topic={topic} parentArticleId={article.parentArticleId} articleId={article.articleId} />
                <ArticleCard article={article} topic={topic} topicList={topicList || []} tagList={tagList || []} />
            </Flex>
        </div>
    );
}

export default TopicPage;