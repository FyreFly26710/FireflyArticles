import "./index.css";
import { Divider, Flex } from "antd";
import Title from "antd/es/typography/Title";
import Link from "antd/es/typography/Link";
import TopicList from "@/components/TopicList";
import ArticleList from "@/components/ArticleList";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { apiArticleGetByPage } from "@/api/contents/api/article";

export const dynamic = 'force-dynamic';

export default async function HomePage() {
    let topicList:any = [];
    let articleList:any = [];
    try {
        const res = await apiTopicGetByPage({
            PageSize: 12,
        });
        topicList = res.data.data ?? [];
    } catch (e:any) {
        console.error("Failed fetching topics, " + e.message);
    }

    try {
        const res = await apiArticleGetByPage({
            PageSize: 12,
        });
        articleList = res.data.data ?? [];
    } catch (e:any) {
        console.error("Failed fetching articles, " + e.message);
    }
    return (
        <div id="homePage" className="max-width-content">
            <Flex justify="space-between" align="center">
                <Title level={3}>Recent Topics</Title>
                <Link href={"/topics"}>Get more</Link>
            </Flex>
            <TopicList topicList={topicList} />
            <Divider />
            <Flex justify="space-between" align="center">
                <Title level={3}>Recent Articles</Title>
                <Link href={"/articles"}>Get more</Link>
            </Flex>
            <ArticleList articleList={articleList} />

        </div>
    );
}