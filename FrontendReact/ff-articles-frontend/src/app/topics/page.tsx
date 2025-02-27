"use server";

import Title from "antd/es/typography/Title";
import "./index.css";
import TopicList from "@/components/TopicList";
import { apiTopicGetByPage } from "@/api/contents/api/topic";

export default async function TopicsPage() {
    const pageSize = 200;
    let topicList:API.TopicDto[]|any = [];
    try {
        const res = await apiTopicGetByPage({
            PageSize: pageSize,
        });
        topicList = res.data.data ?? [];
    } catch (e:any) {
        console.error("Failed fetching topics, " + e.message);
    }
    return (
        <div id="topicsPage" className="max-width-content">
            <Title level={3}>Topics List</Title>
            <TopicList topicList={topicList} />
        </div>
    );
}
