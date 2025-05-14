import { Table, Typography } from 'antd';
import ArticleBriefCard from '../shared/ArticleBriefCard';

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
      title: 'Articles',
      key: 'article',
      render: (_: unknown, record: API.ArticleDto) => (
        <ArticleBriefCard article={record} />
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