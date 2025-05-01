"use client";
import React from 'react';
import { Card, Col, Row, Typography, Space, Divider, Tag, Empty, Avatar } from 'antd';
import { useRouter } from 'next/navigation';

const { Title, Text, Paragraph } = Typography;

interface CategoryTopicsProps {
    topicsByCategory: Record<string, API.TopicDto[]>;
    }

const CategoryTopics: React.FC<CategoryTopicsProps> = ({ topicsByCategory }) => {
    const router = useRouter();
    const categories = Object.keys(topicsByCategory).sort();

    const handleTopicClick = (topicId: number | undefined) => {
        if (topicId) {
            router.push(`/topic/${topicId}`);
        }
    };

    if (Object.keys(topicsByCategory).length === 0) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <Empty description="No topics found" />
            </div>
        );
    }

    return (
        <div style={{ padding: '0 8px' }}>
            {categories.map((category) => (
                <div key={category} className="mb-8">
                    <Divider orientation="left">{category}</Divider>
                    
                    <Row gutter={[16, 16]}>
                        {topicsByCategory[category].map((topic) => (
                            <Col 
                                xs={24} 
                                sm={12} 
                                md={8} 
                                lg={6} 
                                key={topic.topicId}
                            >
                                <Card 
                                    hoverable 
                                    style={{ 
                                        height: '100px',
                                        width: '100%',
                                        padding: '0 8px'
                                    }}
                                    onClick={() => handleTopicClick(topic.topicId)}
                                >
                                    <div className="flex items-start">
                                        <Avatar 
                                            src={topic.topicImage}
                                            size={40}
                                            shape="square"
                                            style={{ marginRight: '12px', flexShrink: 0 }}
                                        />
                                        <div className="overflow-hidden">
                                            <Text strong ellipsis>{topic.title}</Text>
                                            <Paragraph 
                                                ellipsis={{ rows: 2 }}
                                                className="text-xs m-0 mt-1"
                                            >
                                                {topic.abstract || "No description available"}
                                            </Paragraph>
                                        </div>
                                    </div>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </div>
            ))}
        </div>
    );
};

export default CategoryTopics; 