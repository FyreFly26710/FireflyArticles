"use client";
import { Card } from "antd";
import Title from "antd/es/typography/Title";
import "./index.css";
import TagList from "../TagList";
import MdViewer from "../MdViewer";

interface Props {
    article: API.ArticleDto;
}


const ArticleCard = (props: Props) => {
    const { article } = props;

    return (
        <div className="article-card max-width-content">
            <Card bordered={false} className="header-card" >
                <Title level={1} style={{ fontSize: 24 }}>
                    {article.title}
                </Title>
                <TagList tagList={article.tags} />
                <div style={{ marginBottom: 16 }} />
                <MdViewer value={article.content} />
            </Card>
            <div style={{ marginBottom: 16 }} />
            <Card bordered={false} className="content-card">
                <MdViewer value={article.content} />
            </Card>
        </div>
    );
};

export default ArticleCard;
