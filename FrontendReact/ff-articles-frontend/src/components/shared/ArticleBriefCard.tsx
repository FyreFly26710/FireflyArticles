"use client";
import { Tag, Space } from "antd";
import Link from "next/link";

interface Props {
    article: API.ArticleDto;
}

const ArticleBriefCard = ({ article }: Props) => {
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
        <div className="w-full mb-2">
            <div className="flex items-center justify-between w-full">
                <Link
                    href={`/topic/${article.topicId}/article/${article.articleId}`}
                    className="text-lg font-medium mr-2 text-black hover:text-black"
                    style={{ color: '#000000', textDecoration: 'none' }}
                >
                    {article.title}
                </Link>
                {renderTags(article.tags)}
            </div>
            <div className="text-gray-500 mt-1 clearfix">
                {article.abstract? article.abstract : ""}
                <Tag color="green" style={{ float: 'right', marginLeft: 8 }}>Topic: {article.topicTitle}</Tag>
            </div>
        </div>
    );
};

export default ArticleBriefCard;
