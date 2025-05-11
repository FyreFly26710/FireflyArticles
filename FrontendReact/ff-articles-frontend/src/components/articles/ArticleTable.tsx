import { Table, Typography, Tag, Space, Avatar } from 'antd';
import Link from 'next/link';
import { UserOutlined } from '@ant-design/icons';

const { Text, Title, Paragraph } = Typography;

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
            <div
              className="mb-1 text-black hover:text-blue-800"
              style={{
                overflow: 'hidden',
                textOverflow: 'ellipsis',
                whiteSpace: 'nowrap',
                fontWeight: 600,
                fontSize: '16px',
                maxWidth: '280px'
              }}
              title={record.title || 'Untitled Article'}
            >
              {record.title || 'Untitled Article'}
            </div>
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
        <Paragraph
          ellipsis={{ rows: 2, tooltip: record.abstract }}
          style={{ marginBottom: 0 }}
        >
          {record.abstract}
        </Paragraph>
      ),
    },
    {
      title: 'Topic',
      key: 'topic',
      width: 100,
      render: (_: unknown, record: API.ArticleDto) => (
        <Text ellipsis={{ tooltip: record.topicTitle }}>
          {record.topicTitle || 'Uncategorized'}
        </Text>
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
      className="table-with-padding"
      style={{ padding: '8px 16px', background: '#fff', borderRadius: '8px' }}
    />
  );
};

export default ArticleTable; 