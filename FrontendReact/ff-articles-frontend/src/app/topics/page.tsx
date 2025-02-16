"use server";

import Title from "antd/es/typography/Title";
import "./index.css";
import { postTopicGetPage } from "@/api/contents/api/topic";
import TopicList from "@/components/TopicList";

export default async function TopicsPage() {
    const pageSize = 200;
    let topicList:any = [];
    try {
        const res = await postTopicGetPage({
            pageSize: pageSize,
        });
        topicList = res.data.data ?? [];
    } catch (e:any) {
        console.error("Failed fetching topics, " + e.message);
    }
    return (
        <div id="banksPage" className="max-width-content">
            <Title level={3}>Topics List</Title>
            <TopicList topicList={topicList} />
        </div>
    );
}
