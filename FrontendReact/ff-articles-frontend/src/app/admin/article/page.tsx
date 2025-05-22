"use client";
import { Button, Table, Space, Popconfirm, Typography, Input, Card, Row, Col, Tag, Image } from "antd";
import type { ColumnsType, TablePaginationConfig } from "antd/es/table";
import type { SorterResult } from "antd/es/table/interface";
import { PlusOutlined, SearchOutlined } from "@ant-design/icons";
import ArticleFormModal from "@/components/shared/ArticleFormModal";
import { useAdminArticle } from "@/hooks/admin/useAdminArticle";

const { Title } = Typography;

const AdminArticlePage = () => {
    const {
        articles,
        loading,
        pagination,
        searchText,
        editModalVisible,
        currentArticle,
        handleTableChange,
        handleSearchTextChange,
        triggerSearch,
        handleDeleteArticle,
        showEditModal,
        handleEditModalCancel,
        handleEditModalSuccess,
    } = useAdminArticle();

    const columns: ColumnsType<API.ArticleDto> = [
        {
            title: "ID",
            dataIndex: "articleId",
            key: "articleId",
            width: 80,
            hidden: true,
        },
        {
            title: "Title",
            dataIndex: "title",
            key: "title",
            ellipsis: true,
            sorter: true,
        },
        {
            title: "Abstract",
            dataIndex: "abstract",
            key: "abstract",
            ellipsis: true,
            render: (text: string) => (text && text.length > 50 ? `${text.substring(0, 50)}...` : text),
        },
        {
            title: "Topic",
            dataIndex: "topicTitle",
            key: "topicTitle",
            width: 200,
            ellipsis: true,
            sorter: true,
        },
        {
            title: "Tags",
            dataIndex: "tags",
            key: "tags",
            width: 250,
            render: (tags: API.TagDto[] | string[]) => (
                <Space wrap size={[0, 8]}>
                    {tags && tags.map(tag => {
                        const tagName = typeof tag === 'string' ? tag : tag.tagName;
                        const tagColor = typeof tag === 'string' ? 'blue' : (tag.tagColour || 'blue');
                        return (
                            <Tag color={tagColor} key={tagName} style={{ margin: '2px' }}>
                                {tagName}
                            </Tag>
                        );
                    })}
                </Space>
            ),
            ellipsis: true,
        },
        {
            title: "Sort",
            dataIndex: "sortNumber",
            key: "sortNumber",
            width: 80,
            align: 'center',
            sorter: true,
        },
        {
            title: "Type",
            dataIndex: "articleType",
            key: "articleType",
            width: 120,
            sorter: true,
        },
        {
            title: "Hidden",
            dataIndex: "isHidden",
            key: "isHidden",
            width: 100,
            align: 'center',
            render: (value: number | boolean) =>
                value === 1 || value === true ? <Tag color="orange">Hidden</Tag> : <Tag color="green">Visible</Tag>,
            filters: [
                { text: 'Visible', value: 0 },
                { text: 'Hidden', value: 1 },
            ],
        },
        {
            title: "Actions",
            key: "actions",
            width: 120,
            fixed: 'right',
            align: 'center',
            render: (_: any, record: API.ArticleDto) => (
                <Space size="small">
                    <Typography.Link onClick={() => showEditModal(record)}>
                        Edit
                    </Typography.Link>
                    <Popconfirm
                        title="Delete Article"
                        description="Are you sure you want to delete this article?"
                        onConfirm={() => record.articleId && handleDeleteArticle(record.articleId)}
                        okText="Yes, Delete"
                        cancelText="No"
                        okButtonProps={{ danger: true }}
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
        <div className="p-4 md:p-6 bg-gray-50 min-h-screen">
            <Title level={3} className="mb-4 md:mb-6">Article Management</Title>

            <Card bordered={false} className="shadow-sm mb-4 md:mb-6">
                <Row gutter={[16, 16]} justify="space-between" align="middle">
                    <Col xs={24} sm={12} md={10} lg={8} xl={6}>
                        <Input
                            placeholder="Search by title or content"
                            value={searchText}
                            onChange={e => handleSearchTextChange(e.target.value)}
                            onPressEnter={triggerSearch}
                            suffix={<SearchOutlined onClick={triggerSearch} className="cursor-pointer text-gray-400 hover:text-blue-500 transition-colors" />}
                            allowClear
                        />
                    </Col>
                </Row>
            </Card>

            <Card bordered={false} className="shadow-sm">
                <Table
                    rowKey="articleId"
                    columns={columns}
                    dataSource={articles}
                    pagination={{
                        ...pagination,
                        showSizeChanger: true,
                        showTotal: (total, range) => `${range[0]}-${range[1]} of ${total} items`,
                    }}
                    loading={loading}
                    onChange={(p: TablePaginationConfig, f: any, s: SorterResult<API.ArticleDto> | SorterResult<API.ArticleDto>[]) => {
                        const singleSorter = Array.isArray(s) ? s[0] : s;
                        handleTableChange(p, f, singleSorter || {});
                    }}
                    scroll={{ x: 1300 }}
                    className="admin-article-table"
                />
            </Card>

            {editModalVisible && (
                <ArticleFormModal
                    visible={editModalVisible}
                    currentArticle={currentArticle}
                    onCancel={handleEditModalCancel}
                    onSuccess={handleEditModalSuccess}
                />
            )}

            {/* {addModalVisible && (
        <AddArticleModal
          visible={addModalVisible}
          onCancel={handleAddModalCancel}
          onSuccess={handleAddModalSuccess}
        />
      )} */}
        </div>
    );
};

export default AdminArticlePage;
