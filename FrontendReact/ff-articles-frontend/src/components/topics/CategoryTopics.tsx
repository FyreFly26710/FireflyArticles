"use client";
import React from 'react';
import { Col, Row, Divider, Empty, List } from 'antd';
import TopicCard from '../shared/TopicCard';

interface CategoryTopicsProps {
    topicsByCategory: Record<string, API.TopicDto[]>;
}

const CategoryTopics: React.FC<CategoryTopicsProps> = ({ topicsByCategory }) => {
    const categories = Object.keys(topicsByCategory).sort();

    if (Object.keys(topicsByCategory).length === 0) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <Empty description="No topics found" />
            </div>
        );
    }

    return (
        <div>
            {categories.map((category) => (
                <div key={category} className="mb-8">
                    <Divider orientation="left">{category}</Divider>
                    <List
                        grid={{ gutter: 16, column: 4, xs: 1, sm: 2, md: 3, lg: 3, }}
                        dataSource={topicsByCategory[category]}
                        renderItem={(item: API.TopicDto) => (
                            <List.Item>
                                <TopicCard topic={item} />
                            </List.Item>
                        )}
                    />
                </div>
            ))}
        </div>
    );
};

export default CategoryTopics; 