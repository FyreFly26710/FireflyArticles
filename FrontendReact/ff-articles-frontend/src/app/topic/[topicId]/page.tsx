// @ts-nocheck

"use client";
import Title from "antd/es/typography/Title";
import { Avatar, Button, Card } from "antd";
import Meta from "antd/es/card/Meta";
import Paragraph from "antd/es/typography/Paragraph";
import "./index.css";
import { apiTopicGetById } from "@/api/contents/api/topic";
import ArticleList from "@/components/ArticleList";

export default async function BankPage({ params }: { params: { topicId: number } }) {

    const { topicId } = params;

    let topic = undefined;

    try {
        const topicRes = await apiTopicGetById({
            id: (topicId),
        });
        topic = topicRes.data;
        console.log(topic);
    } catch (e: any) {
        console.error("Failed fetching topics, " + e.message);
    }

    if (!topic) {
        return <div>Failed fetching topics, please refresh.</div>;
    }

    let firstArticle: API.ArticleDto | undefined = undefined;
    if (topic.articles && topic.articles.length > 0) {
        firstArticle = topic.articles[0];
    }

    return (
        <div id="bankPage" className="max-width-content">
            <Card>
                <Meta
                    avatar={<Avatar src={topic?.topicImage} size={72} />}
                    title={
                        <Title level={3} style={{ marginBottom: 0 }}>
                            {topic.title}
                        </Title>
                    }
                    description={
                        <>
                            <Paragraph type="secondary">{topic.abstraction}</Paragraph>
                            <Button type="primary"
                                shape="round"
                                href={
                                    firstArticle
                                        ? `/topic/${topicId}/article/${firstArticle.articleId}`
                                        : '/'
                                }
                                target="_blank"
                                disabled={!firstArticle}
                            >
                                Begin
                            </Button>
                        </>
                    }
                ></Meta>
            </Card>
            <div style={{ marginBottom: 16 }} />
            <ArticleList
                articleList={topic.articles || []}
                cardTitle={`Question List (${topic.articles?.length || 0})`}
            />
        </div>
    );
}
