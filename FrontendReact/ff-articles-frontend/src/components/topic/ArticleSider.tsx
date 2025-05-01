"use client";

import { Layout, Menu, Button, Typography } from 'antd';
import { MenuFoldOutlined, MenuUnfoldOutlined } from '@ant-design/icons';
import { useState } from 'react';
import Link from 'next/link';

const { Sider } = Layout;
const { Title } = Typography;

interface ArticleSiderProps {
  topic: API.TopicDto;
  collapsed: boolean;
  onCollapse: () => void;
}

const ArticleSider = ({ topic, collapsed, onCollapse }: ArticleSiderProps) => {
  // Process articles for menu items
  const getMenuItems = () => {
    if (!topic.articles || topic.articles.length === 0) {
      return [];
    }

    return topic.articles.map(article => {
      const hasSubArticles = article.subArticles && article.subArticles.length > 0;
      
      return {
        key: `article-${article.articleId}`,
        label: (
          <Link href={`/topic/${topic.topicId}/article/${article.articleId}`}>
            {article.title}
          </Link>
        ),
        children: hasSubArticles
          ? article.subArticles?.map(subArticle => ({
              key: `article-${subArticle.articleId}`,
              label: (
                <Link href={`/topic/${topic.topicId}/article/${subArticle.articleId}`}>
                  {subArticle.title}
                </Link>
              )
            }))
          : undefined
      };
    });
  };

  return (
    <Sider
      theme="light"
      collapsible
      collapsed={collapsed}
      onCollapse={onCollapse}
      trigger={null}
      width={300}
      collapsedWidth={0}
      className="shadow-sm"
      style={{ height: '100vh', position: 'sticky', top: 0, left: 0 }}
    >
      <div className="p-4">
        <Title level={4} className="mb-4 truncate">
          {topic.title || 'Topic Title'}
        </Title>
        <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={onCollapse}
          className="absolute top-4 right-4"
        />
      </div>
      <Menu
        mode="inline"
        defaultSelectedKeys={[]}
        defaultOpenKeys={[]}
        style={{ borderRight: 0 }}
        items={getMenuItems()}
      />
    </Sider>
  );
};

export default ArticleSider; 