"use client";
import { Divider, Flex, Spin } from "antd";
import Title from "antd/es/typography/Title";
import Link from "antd/es/typography/Link";
import TopicList from "@/components/article/TopicList";
import ArticleList from "@/components/article/ArticleList";
import { useHomePageData } from "./hooks";

export default function HomePage() {
    const { topicList, articleList, loading, error } = useHomePageData();

    if (loading) {
        return (
            <div className="flex justify-center items-center h-screen">
                <Spin size="large" />
            </div>
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