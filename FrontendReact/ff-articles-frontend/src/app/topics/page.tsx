"use server";

import Title from "antd/es/typography/Title";
import "./index.css";
import TopicList from "@/components/TopicList";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { Divider } from "antd";

export const dynamic = 'force-dynamic';

// Define category order (lower number = higher priority)
const CATEGORY_ORDER: Record<string, number> = {
    "Programming language": 1,
    "Other": 2
};

function compareCategories(a: string, b: string): number {
    const orderA = CATEGORY_ORDER[a] ?? Number.MAX_SAFE_INTEGER;
    const orderB = CATEGORY_ORDER[b] ?? Number.MAX_SAFE_INTEGER;
    return orderA === orderB ? a.localeCompare(b) : orderA - orderB;
}

export default async function TopicsPage() {
    const pageSize = 200;
    let topicList: API.TopicDto[] = [];
    try {
        const res = await apiTopicGetByPage({
            PageSize: pageSize,
            IncludeContent: false,
            IncludeArticles: false,
            IncludeSubArticles: false,
            IncludeUser: false,
        });
        //@ts-ignore
        topicList = res.data?.data ?? [];
    } catch (e: any) {
        console.error("Failed fetching topics, " + e.message);
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
