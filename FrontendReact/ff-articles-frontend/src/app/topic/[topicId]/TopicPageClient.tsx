"use client";

import { useState } from 'react';
import { Layout, Row, Col, Spin } from 'antd';
import dynamic from 'next/dynamic';

// Dynamically import components to reduce initial bundle size
const ArticleSider = dynamic(() => import('@/components/topic/ArticleSider'), {
  loading: () => <div className="h-full w-64 bg-gray-100" />
});

const ArticleHeaderCard = dynamic(() => import('@/components/topic/ArticleHeaderCard'), {
  loading: () => <div className="h-24 w-full bg-gray-100 rounded" />
});

const ArticleContentCard = dynamic(() => import('@/components/topic/ArticleContentCard'), {
  loading: () => <div className="h-96 w-full bg-gray-100 rounded" />
});

const ArticleButtons = dynamic(() => import('@/components/topic/ArticleButtons'), {
  loading: () => <div />
});

const { Content } = Layout;

interface TopicPageClientProps {
  topic: API.TopicDto | null;
  error: string | null;
}

const TopicPageClient = ({ topic, error }: TopicPageClientProps) => {
  const [siderCollapsed, setSiderCollapsed] = useState(false);

  const toggleSider = () => {
    setSiderCollapsed(!siderCollapsed);
  };

  if (error) {
    return (
      <div className="flex justify-center items-center h-screen">
        <p className="text-red-500">{error}</p>
      </div>
    );
  }

  if (!topic) {
    return (
      <div className="flex justify-center items-center h-screen">
        <p>Topic not found or failed to load.</p>
      </div>
    );
  }

  return (
    <Layout className="min-h-screen">
      <ArticleSider 
        topic={topic} 
        collapsed={siderCollapsed} 
        onCollapse={toggleSider} 
      />
      <Layout className="site-layout">
        <Content className="p-6">
          <Row gutter={[16, 16]}>
            <Col xs={24} lg={24}>
              <ArticleHeaderCard topic={topic} />
            </Col>
            <Col xs={24} lg={24}>
              <ArticleContentCard topic={topic} />
            </Col>
          </Row>
        </Content>
      </Layout>
      <ArticleButtons topicId={topic.topicId} />
    </Layout>
  );
};

export default TopicPageClient; 