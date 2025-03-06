"use server";
import ArticleCard from "@/app/topic/components/ArticleCard";
import ArticleSider from "@/app/topic/components/ArticleSider";
import { fetchArticle, fetchTags, fetchTopic, fetchTopicArticle, fetchTopicList } from "@/app/topic/utils/fetcher";
import { Flex } from "antd";

export default async function TopicPage({ params }: { params: { topicId: number} }) {
    const { topicId } = params;

    const topic = await fetchTopic(topicId);
    if (!topic) return <div>Failed fetching topics, please refresh page.</div>;

    const [topicList, tagList, article] = await Promise.all([
        fetchTopicList(),
        fetchTags(),
        fetchTopicArticle(topic),
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