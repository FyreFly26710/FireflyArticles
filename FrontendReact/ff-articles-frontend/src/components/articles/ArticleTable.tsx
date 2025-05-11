import { Table, Typography, Tag, Space, Avatar } from 'antd';
import Link from 'next/link';
import { UserOutlined } from '@ant-design/icons';

const { Text, Title } = Typography;

interface ArticleTableProps {
  articles: API.ArticleDto[];
  loading: boolean;
  total: number;
  pageNumber: number;
  pageSize: number;
  onPageChange: (page: number, pageSize?: number) => void;
}

const ArticleTable = ({
  articles,
  loading,
  total,
  pageNumber,
  pageSize,
  onPageChange
}: ArticleTableProps) => {
  const columns = [
    {
      title: 'Article',
      key: 'article',
      width: 300,
      render: (_: unknown, record: API.ArticleDto) => (
        <div>
          <Link href={`/topic/${record.topicId}/article/${record.articleId}`}>
            <Title level={5} className="mb-1 text-blue-600 hover:text-blue-800">
              {record.title || 'Untitled Article'}
            </Title>
          </Link>
          <Space size={[0, 4]} wrap className="mb-1">
            {record.tags?.map((tag, index) => (
              <Tag key={index} color="blue" className="mb-0">
                {tag}
              </Tag>
            ))}
          </Space>
        </div>
      ),
    },
    {
        title: 'Description',
        key: 'abstract',
        width: 300,
        render: (_: unknown, record: API.ArticleDto) => (
            <Text>{record.abstract}</Text>
        ),
    },
    {
      title: 'Topic',
      key: 'topic',
      width: 150,
      render: (_: unknown, record: API.ArticleDto) => (
        <Text>{record.topicTitle || 'Uncategorized'}</Text>
      ),
    },
    {
      title: 'Author',
      key: 'author',
      width: 150,
      render: (_: unknown, record: API.ArticleDto) => (
        <Space>
          <Avatar 
            size="small" 
            src={record.user?.userAvatar} 
            icon={<UserOutlined />} 
          />
          <Text>{record.user?.userName || 'Unknown'}</Text>
        </Space>
      ),
    },
  ];

  return (
    <Table
      rowKey="articleId"
      dataSource={articles}
      columns={columns}
      loading={loading}
      pagination={{
        current: pageNumber,
        pageSize,
        total,
        onChange: onPageChange,
        showSizeChanger: true,
        pageSizeOptions: ['10', '20', '50'],
        showTotal: (total) => `Total ${total} articles`,
      }}
      size="small"
    />
  );
};

export default ArticleTable; 