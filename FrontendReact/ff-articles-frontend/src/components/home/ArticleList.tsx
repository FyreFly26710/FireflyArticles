"use client";
import { Card, List } from "antd";
import ArticleBriefCard from "../shared/ArticleBriefCard";

interface Props {
    articleList: API.ArticleDto[];
    cardTitle?: string;
}

const ArticleList = (props: Props) => {
    const { articleList = [], cardTitle } = props;

    return (
        <Card className="article-list" title={cardTitle}>
            <List
                itemLayout="vertical"
                dataSource={articleList}
                renderItem={(item: API.ArticleDto) => (
                    <List.Item>
                        <ArticleBriefCard article={item} />
                    </List.Item>
                )}
            />
        </Card>
    );
};

export default ArticleList;
