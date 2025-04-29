"use client";
import Title from "antd/es/typography/Title";
import TopicList from "@/components/article/TopicList";
import { Divider, Spin } from "antd";
import { useTopics } from "./hooks";

// Define category order (lower number = higher priority)
const CATEGORY_ORDER: Record<string, number> = {
    "FF Wiki": 1,
    "Other": 267100
};

function compareCategories(a: string, b: string): number {
    const orderA = CATEGORY_ORDER[a] ?? 26710;
    const orderB = CATEGORY_ORDER[b] ?? 26710;
    return orderA === orderB ? a.localeCompare(b) : orderA - orderB;
}

const TopicsPage = () => {
    const pageSize = 200;
    const { topicList, loading, error } = useTopics(pageSize);

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
                <Title level={3}>Error Loading Topics</Title>
                <p>Failed to load topics. Please try again later.</p>
            </div>
        );
    }

    // Group topics by category
    const topicsByCategory = topicList.reduce((acc, topic) => {
        const category = topic.category || 'Other';
        if (!acc[category]) {
            acc[category] = [];
        }
        acc[category].push(topic);
        return acc;
    }, {} as Record<string, API.TopicDto[]>);

    const sortedCategories = Object.entries(topicsByCategory)
        .sort(([a], [b]) => compareCategories(a, b));

    return (
        <div id="topicsPage" className="max-width-content">
            <Title level={3}>Topics List</Title>
            {sortedCategories.map(([category, topics]) => (
                <div key={category}>
                    <Divider orientation="left">{category}</Divider>
                    <TopicList topicList={topics} />
                </div>
            ))}
        </div>
    );
}

export default TopicsPage;