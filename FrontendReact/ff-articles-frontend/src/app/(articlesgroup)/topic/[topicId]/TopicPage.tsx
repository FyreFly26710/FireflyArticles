"use server";
import ArticleCard from "@/components/article/ArticleCard";
import ArticleSider from "@/components/article/ArticleSider";
import { fetchArticle, fetchTags, fetchTopic, fetchTopicArticle, fetchTopicList } from "@/app/(articlesgroup)/topic/fetcher";
import { Flex } from "antd";

const TopicPage = async ({ params }: { params: { topicId: number } }) => {
    const { topicId } = params;

    const topic = await fetchTopic(topicId);
    if (!topic) return <div>Failed fetching topics, please refresh page.</div>;

    const [topicList, tagList] = await Promise.all([
        fetchTopicList(),
        fetchTags(),
    ]);
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