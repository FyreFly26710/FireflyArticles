"use client";
import { Divider, Flex, Spin } from "antd";
import Title from "antd/es/typography/Title";
import Link from "antd/es/typography/Link";
import TopicList from "@/components/home/TopicList";
import ArticleList from "@/components/home/ArticleList";
import { useHomePageData } from "./hooks";
import LoadingSpin from "@/components/shared/LoadingSpin";
export default function HomePage() {
    const { topicList, articleList, loading, error } = useHomePageData();

    if (loading) {
        return (
            <LoadingSpin />
        );
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