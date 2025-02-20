// @ts-nocheck

"use server";
import Link from "next/link";
import {Flex, Menu} from "antd";
import Sider from "antd/es/layout/Sider";
import Title from "antd/es/typography/Title";
import {Content} from "antd/es/layout/layout";
import ArticleCard from "@/components/ArticleCard";
import { apiTopicGetById } from "@/api/contents/api/topic";
import { apiArticleGetById } from "@/api/contents/api/article";

export default async function TopicPage({ params }:{params:{ topicId:number, articleId:number }}) {
    const { topicId, articleId } = params;

    let topic = undefined;
    try {
        const topicRes = await apiTopicGetById({
            id: (topicId),
        });
        topic = topicRes.data;
    } catch (e:any) {
        console.error("Failed fetching topics, " + e.message);
    }
    if (!topic) {
        return <div>Failed fetching topics, please refresh page.</div>;
    }

    let article = undefined;
    try {
        const articleRes = await apiArticleGetById({
            id: (articleId),
        });
        article = articleRes.data;
    } catch (e:any) {
        console.error("Failed fetching questions, " + e.message);
    }
    if (!article) {
        return <div>Failed fetching questions, please refresh page.</div>;
    }

    const questionMenuItemList = (topic.articles || []).map((q:any) => {
        return {
            label: (
                <Link href={`/topic/${topicId}/article/${q.articleId}`}>{q.title}</Link>
            ),
            key: q.articleId,
        };
    });

    return (
        <div id="bankQuestionPage"  className="max-width-content">
                <Flex gap={12}>
                <Sider width={240} theme="light"
                       className="siderBar"
                       collapsible>
                    <Title
                        level={4}
                        style={{
                            padding: "0 20px",
                            overflow: "hidden",
                            whiteSpace: "nowrap",
                            textOverflow: "ellipsis",
                        }}
                    >
                        {topic.title}
                    </Title>
                    <Menu items={questionMenuItemList} selectedKeys={[String(article.articleId)]} />
                </Sider>
                <Content  >
                    <ArticleCard
                        article={article}>
                    </ArticleCard>
                </Content>
            </Flex>
        </div>
    );
}
