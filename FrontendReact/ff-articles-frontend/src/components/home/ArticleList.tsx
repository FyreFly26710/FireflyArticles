"use client";
import { Card, List, Tag, Space } from "antd";
import Link from "next/link";

interface Props {
    articleList: API.ArticleDto[];
    cardTitle?: string;
}

const ArticleList = (props: Props) => {
    const { articleList = [], cardTitle } = props;

    // Format tags to be displayed on the right side of the title
    const renderTags = (tags: string[] = []) => {
        if (!tags || tags.length === 0) return null;

        return (
            <Space size={4} style={{ marginLeft: 'auto', flexWrap: 'wrap', justifyContent: 'flex-end' }}>
                {tags.map((tag) => (
                    <Tag key={tag} color="blue" style={{ margin: 0 }}>{tag}</Tag>
                ))}
            </Space>
        );
    };

    return (
        <Card className="article-list" title={cardTitle}>
            <List
                itemLayout="vertical"
                dataSource={articleList}
                renderItem={(item: API.ArticleDto) => (
                    <List.Item>
                        <div className="w-full mb-2">
                            <div className="flex items-center justify-between w-full">
                                <Link
                                    href={`/topic/${item.topicId}/article/${item.articleId}`}
                                    className="text-lg font-medium mr-2 text-black hover:text-black"
                                    style={{ color: '#000000', textDecoration: 'none' }}
                                >
                                    {item.title}
                                </Link>
                                {renderTags(item.tags)}
                            </div>
                        </div>
                        {item.abstract && (
                            <div className="text-gray-500 mt-1">
                                {item.abstract}
                            </div>
                        )}
                    </List.Item>
                )}
            />
        </Card>
    );
};

export default ArticleList;
