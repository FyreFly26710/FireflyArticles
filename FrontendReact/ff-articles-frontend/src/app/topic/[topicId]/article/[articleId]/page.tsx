
"use server";
import Link from "next/link";
import { Button, ConfigProvider, Flex, Layout, Menu, MenuProps, Popover, Tooltip } from "antd";
import Sider from "antd/es/layout/Sider";
import Title from "antd/es/typography/Title";
import { Content } from "antd/es/layout/layout";
import ArticleCard from "@/components/ArticleCard";
import { apiTopicGetById } from "@/api/contents/api/topic";
import { apiArticleGetById } from "@/api/contents/api/article";
import { Children } from "react";
import { FileAddOutlined, PlusCircleOutlined, PlusSquareOutlined } from "@ant-design/icons";

export default async function ArticlePage({ params }: { params: { topicId: number, articleId: number } }) {
    const { topicId, articleId } = params;

    let topic: API.TopicDto | undefined;
    try {
        const topicRes = await apiTopicGetById({
            id: (topicId),
        });
        // @ts-ignore
        topic = topicRes.data;
        console.log(topic);
    } catch (e: any) {
        console.error("Failed fetching topics, " + e.message);
    }
    if (!topic) {
        return <div>Failed fetching topics, please refresh page.</div>;
    }

    let article: API.ArticleDto | undefined;
    try {
        // get topic article
        // if (articleId === 0) {
        //     articleId = topic.articles[0].articleId;
        // }
        const articleRes = await apiArticleGetById({
            id: (articleId),
        });
        // @ts-ignore
        article = articleRes.data;
    } catch (e: any) {
        console.error("Failed fetching articles, " + e.message);
    }
    if (articleId === 0) {
        article = { title: topic.title, abstraction: topic.abstraction, content: "" } as API.ArticleDto;
    }

    if (!article) {
        return <div>Failed fetching articles, please refresh page.</div>;
    }



    type MenuItem = Required<MenuProps>['items'][number];

    function getItem(a: API.ArticleMiniDto): MenuItem {
        return {
            key: a.articleId,
            label: (
                <Popover placement="bottomLeft" content={a.title} arrow={{ pointAtCenter: true }}>
                    <Link href={`/topic/${a.topicId}/article/${a.articleId}`}>{a.title}</Link>
                </Popover>
            ),
            children: a.subArticles && a.subArticles.length > 0
                ? a.subArticles.map((sub) => getItem(sub))
                : undefined,
        } as MenuItem;
    }
    const articleMenuItemList = (topic.articles || []).map(getItem);

    return (
        <div id="articlePage" className="max-width-content">
            <Flex gap={12}>
                <Sider theme="light" breakpoint="lg" collapsedWidth="0" width={220}>
                    <Flex vertical style={{ height: "100%" }}>
                        <Title level={4} style={{ margin: "5px 20px" }}>
                            <Link href={`/topic/${topicId}`} style={{ color: "black" }}>{topic.title}</Link>
                        </Title>

                        {/* TODO: Make theme global */}
                        <ConfigProvider
                            theme={{
                                components: {
                                    Menu: {
                                        subMenuItemBg: "transparent",
                                    },
                                },
                            }}
                        >
                            <Menu
                                items={articleMenuItemList}
                                mode="inline"
                                selectedKeys={[String(article.articleId)]}
                                defaultOpenKeys={[String(article.parentArticleId ?? article.articleId)]}
                            />
                        </ConfigProvider>
                        <div style={{ margin: "auto 20px 20px 20px", textAlign: "right", fontSize: "16px" }}>
                            <Link href={`/topic/${topicId}`} style={{ color: "GrayText" }}>
                                <PlusSquareOutlined />{" "} New Page
                            </Link>
                        </div>
                    </Flex>

                </Sider>
                <ArticleCard
                    article={article}>
                </ArticleCard>
            </Flex>
        </div>
    );
}
