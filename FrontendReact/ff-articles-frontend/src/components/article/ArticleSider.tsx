"use client";

import { Layout, Menu, Button, Typography, Tooltip, Avatar } from 'antd';
import { MenuFoldOutlined, MenuUnfoldOutlined } from '@ant-design/icons';
import Link from 'next/link';
import { useMemo } from 'react';

const { Sider } = Layout;
const { Title } = Typography;

interface ArticleSiderProps {
  topic: API.TopicDto;
  collapsed: boolean;
  onCollapse: () => void;
  articleId: number | null;
}

const ArticleSider = ({ topic, collapsed, onCollapse, articleId }: ArticleSiderProps) => {
  // Determine selected keys based on current articleId
  const selectedKeys = useMemo(() => {
    return articleId ? [`article-${articleId}`] : [];
  }, [articleId]);

  // Find current article for expanded keys
  const expandedKeys = useMemo(() => {
    if (!articleId || !topic.articles) return [];

    // Check if this article is a sub-article and find parent
    for (const article of topic.articles) {
      if (article.subArticles) {
        for (const subArticle of article.subArticles) {
          if (subArticle.articleId === articleId) {
            return [`article-${article.articleId}`];
          }
        }
      }
    }

    return [];
  }, [articleId, topic.articles]);

  // Process articles for menu items
  const getMenuItems = () => {
    if (!topic.articles || topic.articles.length === 0) {
      return [];
    }

    return topic.articles.map(article => {
      const hasSubArticles = article.subArticles && article.subArticles.length > 0;

      return {
        key: `article-${article.articleId}`,
        label: collapsed ? (
          <Tooltip title={article.title} placement="right">
            <Link href={`/topic/${topic.topicId}/article/${article.articleId}`}>
              {article.sortNumber?.toString() || '#'}
            </Link>
          </Tooltip>
        ) : (
          <Link href={`/topic/${topic.topicId}/article/${article.articleId}`}>
            {article.title}
          </Link>
        ),
        children: hasSubArticles
          ? article.subArticles?.map(subArticle => ({
            key: `article-${subArticle.articleId}`,
            label: collapsed ? (
              <Tooltip title={subArticle.title} placement="right">
                <Link href={`/topic/${topic.topicId}/article/${subArticle.articleId}`}>
                  {subArticle.sortNumber?.toString() || '#'}
                </Link>
              </Tooltip>
            ) : (
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
      collapsedWidth={64}
      className="shadow-sm"
      style={{
        height: 'calc(100vh - 64px)',
        position: 'fixed',
        top: 64,
        left: 0,
        overflow: 'hidden',
        marginTop: 0
      }}
    >
      <div className="p-2 pb-0">
        {!collapsed && (
          <Link href={`/topic/${topic.topicId}`} className="flex items-center">
            <Avatar src={topic.topicImage} shape="square" />
            <Title level={4} className="truncate ml-2 mt-1">
              {topic.title || 'Topic Title'}
            </Title>
          </Link>
        )}
      </div>

      {/* Scroll container */}
      <div style={{
        height: collapsed ? 'calc(100% - 60px)' : 'calc(100% - 90px)',
        overflowY: 'auto' as const,
        overflowX: 'hidden' as const,
        width: collapsed ? '64px' : '100%',
        paddingBottom: '60px'
      }}>
        <Menu
          mode="inline"
          selectedKeys={selectedKeys}
          defaultOpenKeys={expandedKeys}
          style={{
            borderRight: 0,
            width: collapsed ? 64 : '100%',
            marginTop: 0
          }}
          items={getMenuItems()}
        />
      </div>

      {/* Fixed bottom button */}
      <div
        style={{
          position: 'absolute',
          bottom: '0',
          width: '100%',
          height: '40px',
          display: 'flex',
          justifyContent: 'center',
          background: 'white',
          padding: '0 0',
          zIndex: 10,
          borderTop: '1px solid #f0f0f0'
        }}
      >
        <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={onCollapse}
          style={{ width: '100%', height: '100%' }}
        />
      </div>
    </Sider>
  );
};

export default ArticleSider; 