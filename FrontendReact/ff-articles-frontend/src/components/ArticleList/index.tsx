"use client";
import { Card, List, Tag } from "antd";
import Link from "next/link";

interface Props {
    articleList: API.ArticleDto[];
    cardTitle?: string;
}

const ArticleList = (props: Props) => {
    const { articleList = [], cardTitle } = props;
    const tagList = (tags: string[] = []) => {
        return tags.map((tag) => {
            return <Tag key={tag}>{tag}</Tag>;
        });
    };

    return (
        <Card className="article-list" title={cardTitle}>
            <List
                dataSource={articleList}
                renderItem={(item: API.ArticleDto) => (
                    <List.Item extra={tagList(item.tags)}>
                        <List.Item.Meta
                            title={<Link
                                href={`/topic/${item.topicId}/article/${item.articleId}`}>
                                {item.title}
                            </Link>}
                            description={item.abstract}
                        />
                    </List.Item>
                )}
            />
        </Card>
    );
};

export default ArticleList;
