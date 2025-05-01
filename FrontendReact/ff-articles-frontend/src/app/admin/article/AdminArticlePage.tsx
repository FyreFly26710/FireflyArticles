"use client";
import { Button, Table, Space, Popconfirm, Typography, Input, Card, Row, Col, Tag } from "antd";
import { useEffect, useState } from "react";
import { PlusOutlined, SearchOutlined, TagsOutlined } from "@ant-design/icons";
import ArticleFormModal from "@/components/shared/ArticleFormModal";
import AddArticleModal from "@/components/admin/AddArticleModal";
import { apiArticleGetByPage, apiArticleDeleteById } from "@/api/contents/api/article";
import { message } from "antd";

const { Title } = Typography;

const AdminArticlePage = () => {
  // Table data state
  const [articles, setArticles] = useState<API.ArticleDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });

  // Search state
  const [searchText, setSearchText] = useState('');

  // Edit Modal states
  const [editModalVisible, setEditModalVisible] = useState(false);
  const [currentArticle, setCurrentArticle] = useState<API.ArticleDto | null>(null);

  // Add Modal state
  const [addModalVisible, setAddModalVisible] = useState(false);

  // Parent articles for Add Modal only
  const [parentArticles, setParentArticles] = useState<API.ArticleDto[]>([]);

  // Fetch articles with pagination and search
  const fetchArticles = async (page = pagination.current, pageSize = pagination.pageSize, keyword = searchText) => {
    setLoading(true);
    try {
      const response = await apiArticleGetByPage({
        PageNumber: page,
        PageSize: pageSize,
        Keyword: keyword || undefined,
        IncludeUser: true,
      });

      if (response.data?.data) {
        setArticles(response.data.data);
        setPagination({
          ...pagination,
          current: page,
          pageSize,
          total: response.data.counts || 0,
        });
      }
    } catch (error) {
      console.error("Failed to fetch articles:", error);
      message.error("Failed to load articles");
    } finally {
      setLoading(false);
    }
  };

  // Fetch parent articles for AddArticleModal
  const fetchParentArticles = async () => {
    try {
      const response = await apiArticleGetByPage({
        PageSize: 100,
        PageNumber: 1,
      });

      const articles = response.data?.data || [];
      const parentArticles = articles.filter(article => article.articleType === 'Article');
      setParentArticles(parentArticles);
    } catch (error) {
      console.error("Failed to fetch parent articles:", error);
    }
  };

  // Fetch initial data
  useEffect(() => {
    fetchArticles();
    fetchParentArticles();
  }, []);

  // Handle table pagination change
  const handleTableChange = (pagination: any) => {
    fetchArticles(pagination.current, pagination.pageSize);
  };

  // Handle search
  const handleSearch = () => {
    setPagination({ ...pagination, current: 1 });
    fetchArticles(1, pagination.pageSize, searchText);
  };

  // Handle delete article
  const handleDelete = async (articleId: number) => {
    try {
      const response = await apiArticleDeleteById({ id: articleId });
      if (response.data) {
        message.success("Article deleted successfully");
        fetchArticles();
      } else {
        message.error(response.message || "Failed to delete article");
      }
    } catch (error) {
      console.error("Delete error:", error);
      message.error("Failed to delete article");
    }
  };

  // Open modal for adding new article
  const showAddModal = () => {
    setAddModalVisible(true);
  };

  // Open modal for editing an article
  const showEditModal = (article: API.ArticleDto) => {
    setCurrentArticle(article);
    setEditModalVisible(true);
  };

  // Handle modal success (add or edit)
  const handleModalSuccess = () => {
    setEditModalVisible(false);
    setAddModalVisible(false);
    fetchArticles();
  };

  // Table columns
  const columns = [
    {
      title: "ID",
      dataIndex: "articleId",
      key: "articleId",
      width: 80,
    },
    {
      title: "Title",
      dataIndex: "title",
      key: "title",
      ellipsis: true,
    },
    {
      title: "Abstract",
      dataIndex: "abstract",
      key: "abstract",
      ellipsis: true,
      render: (text: string) => text && text.length > 50 ? `${text.substring(0, 50)}...` : text
    },
    {
      title: "Topic",
      dataIndex: "topicTitle",
      key: "topicTitle",
      width: 300,
      ellipsis: true,
    },
    {
      title: "Tags",
      dataIndex: "tags",
      key: "tags",
      width: 300,
      render: (tags: string[]) => (
        <span>
          {tags && tags.map(tag => (
            <Tag color="blue" key={tag} style={{ margin: '2px' }}>
              {tag}
            </Tag>
          ))}
        </span>
      ),
      ellipsis: true,
    },
    {
      title: "Sort",
      dataIndex: "sortNumber",
      key: "sortNumber",
      width: 80,
      sorter: (a: API.ArticleDto, b: API.ArticleDto) =>
        (a.sortNumber || 0) - (b.sortNumber || 0),
    },
    {
      title: "Type",
      dataIndex: "articleType",
      key: "articleType",
      width: 150,
    },
    {
      title: "Hidden",
      dataIndex: "isHidden",
      key: "isHidden",
      width: 80,
      render: (value: number) =>
        value === 1 ? <Tag color="red">Hidden</Tag> : <Tag color="green">Visible</Tag>
    },
    {
      title: "Actions",
      key: "actions",
      width: 120,
      render: (_: any, record: API.ArticleDto) => (
        <Space size="small">
          <Typography.Link onClick={() => showEditModal(record)}>
            Edit
          </Typography.Link>
          <Popconfirm
            title="Are you sure you want to delete this article?"
            onConfirm={() => handleDelete(record.articleId)}
            okText="Yes"
            cancelText="No"
          >
            <Typography.Link type="danger">
              Delete
            </Typography.Link>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-6">
      <Title level={3}>Article Management</Title>

      <Card className="mb-4">
        <Row gutter={16} className="mb-4">
          <Col span={8}>
            <Input
              placeholder="Search by title or content"
              value={searchText}
              onChange={e => setSearchText(e.target.value)}
              onPressEnter={handleSearch}
              suffix={<SearchOutlined onClick={handleSearch} style={{ cursor: 'pointer' }} />}
            />
          </Col>
          <Col span={16} className="text-right">
            <Space>
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={showAddModal}
              >
                Add Article
              </Button>
            </Space>
          </Col>
        </Row>

        <Table
          rowKey="articleId"
          columns={columns}
          dataSource={articles}
          pagination={{
            ...pagination,
            showSizeChanger: true,
            showTotal: (total) => `Total ${total} items`,
          }}
          loading={loading}
          onChange={handleTableChange}
          scroll={{ x: 1200 }}
        />
      </Card>

      {/* Edit Modal */}
      <ArticleFormModal
        visible={editModalVisible}
        currentArticle={currentArticle}
        onCancel={() => setEditModalVisible(false)}
        onSuccess={handleModalSuccess}
      />

      {/* Add Modal */}
      <AddArticleModal
        visible={addModalVisible}
        onCancel={() => setAddModalVisible(false)}
        onSuccess={handleModalSuccess}
      />
    </div>
  );
};

export default AdminArticlePage;
