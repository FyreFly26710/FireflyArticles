import { Divider, Flex } from "antd";
import Title from "antd/es/typography/Title";
import Link from "antd/es/typography/Link";
import TopicList from "@/components/home/TopicList";
import ArticleList from "@/components/home/ArticleList";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { apiArticleGetByPage } from "@/api/contents/api/article";

export const dynamic = 'force-dynamic';

export default async function Page() {
    let topics: API.TopicDto[] = [];
    let articles: API.ArticleDto[] = [];
    let error = null;

    try {
        const [topicResponse, articleResponse] = await Promise.all([
            apiTopicGetByPage({
                PageSize: 12,
            }),
            apiArticleGetByPage({
                PageSize: 10,
            })
        ]);

        if (topicResponse.data) {
            topics = topicResponse.data.data ?? [];
        }

        if (articleResponse.data) {
            articles = articleResponse.data.data ?? [];
        }
    } catch (err) {
        error = err;
        console.error("Failed to load homepage data:", err);
    }

    if (error) {
        return (
            <div className="max-width-content">
                <Title level={3}>Error Loading Data</Title>
                <p>Failed to load homepage data. Please try again later.</p>
            </div>
        );
    }

    return (
        <div id="homePage" className="max-width-content">
            <Flex justify="space-between" align="center">
                <Title level={3}>Recent Topics</Title>
                <Link href={"/topics"}>Get more</Link>
            </Flex>
            <TopicList topicList={topics} />
            <Divider />
            <Flex justify="space-between" align="center">
                <Title level={3}>Recent Articles</Title>
                <Link href={"/articles"}>Get more</Link>
            </Flex>
            <ArticleList articleList={articles} />
        </div>
    );
}