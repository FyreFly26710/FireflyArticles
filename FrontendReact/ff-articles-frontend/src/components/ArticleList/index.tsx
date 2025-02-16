"use client";
import {Card, List, Tag} from "antd";
import "./index.css";
import Link from "next/link";

interface Props {
    topicId?: number;
    articleList: API.ArticleResponse[];
    cardTitle?: string;
}

const ArticleList = (props: Props) => {
    const {articleList = [], cardTitle, topicId} = props;

    const tagList = (tags: string[] = []) => {
        return tags.map((tag) => {
            return <Tag key={tag}>{tag}</Tag>;
        });
    };

    return (
        <Card className="question-list" title={cardTitle}>
            <List
                dataSource={articleList}
                renderItem={(item: API.ArticleResponse) => (
                    <List.Item extra={tagList(item.tags)}>
                        <List.Item.Meta
                            title={<Link
                                href={topicId ? `/bank/${topicId}/question/${item.articleId}` : `/question/${item.articleId}`}>
                                {item.title}
                            </Link>}
                        />
                    </List.Item>
                )}
            />
        </Card>
    );
};

export default ArticleList;
