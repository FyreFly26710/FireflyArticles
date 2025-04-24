"use client";

import { MenuFoldOutlined, MenuUnfoldOutlined, PlusSquareOutlined } from "@ant-design/icons";
import { Avatar, Button, ConfigProvider, Flex, Menu, MenuProps, Typography } from "antd";
import Sider from "antd/es/layout/Sider";
import Link from "next/link";
import { useState } from "react";

const { Title } = Typography;

type ArticleSiderProps = {
    topic: API.TopicDto;
    parentArticleId: number | undefined;
    articleId: number;
};

export default function ArticleSider({ topic, parentArticleId, articleId }: ArticleSiderProps) {
    const [collapsed, setCollapsed] = useState(false);

    type MenuItem = Required<MenuProps>["items"][number];

    function getItem(a: API.ArticleDto): MenuItem {
        return {
            key: String(a.articleId),
            label: (
                <Link href={`/topic/${a.topicId}/article/${a.articleId}`}>{a.title}</Link>
            ),
            children: a.subArticles?.length ? a.subArticles.map(getItem) : undefined,
        };
    }

    const articleMenuItemList = (topic.articles || []).map(getItem);

    return (
        <Sider theme="light" breakpoint="lg" collapsedWidth="0" width={220} collapsible
            zeroWidthTriggerStyle={{ top: 0 }}>
            <Flex vertical style={{ height: "100%" }}>

                <Title level={4} style={{ margin: "5px 20px" }}>
                    <Link href={`/topic/${topic.topicId}`} style={{ color: "black" }}>
                        {topic.title}
                    </Link>
                </Title>
                {/* todo: menu only allow one open. Expand to see full text */}
                <ConfigProvider theme={{ components: { Menu: { subMenuItemBg: "transparent" } } }}>
                    <Menu
                        items={articleMenuItemList}
                        mode="inline"
                        selectedKeys={[String(articleId)]}
                        defaultOpenKeys={[String(parentArticleId && parentArticleId !== 0 ? parentArticleId : articleId)]}
                    />
                </ConfigProvider>
                <div style={{ margin: "auto 20px 20px 20px", textAlign: "right", fontSize: "16px" }}>
                    <Link href={`/topic/${topic.topicId}/new?redirectId=0`} style={{ color: "GrayText" }}>
                        <PlusSquareOutlined /> New Page
                    </Link>
                </div>
            </Flex>
        </Sider>
    );
} 