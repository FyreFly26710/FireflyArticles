"use client";
import { Button, Table, Input, Card, Row, Col, Typography, Space, Tag, Popconfirm, Image } from "antd";
import { PlusOutlined, SearchOutlined } from "@ant-design/icons";
import type { ColumnsType, TablePaginationConfig } from "antd/es/table";
import TopicFormModal from "@/components/admin/TopicFormModal";
import { useAdminTopic } from "@/hooks/admin/useAdminTopic";

const { Title } = Typography;

const AdminTopicPage = () => {
    const {
        topics,
        loading,
        pagination,
        searchText,
        modalVisible,
        modalMode,
        currentTopic,
        handleTableChange,
        handleSearchTextChange,
        triggerSearch,
        showAddModal,
        showEditModal,
        handleModalCancel,
        handleModalSuccess,
        handleDeleteTopic,
    } = useAdminTopic();

    const columns: ColumnsType<API.TopicDto> = [
        {
            title: "ID",
            dataIndex: "topicId",
            key: "topicId",
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
            render: (text: string) => text && text.length > 50 ? `${text.substring(0, 50)}...` : text,
        },
        {
            title: "Category",
            dataIndex: "category",
            key: "category",
            sorter: true,
            ellipsis: true,
        },
        {
            title: "Image",
            dataIndex: "topicImage",
            key: "topicImage",
            width: 100,
            align: 'center',
            render: (text: string) => text ? (
                <Image
                    src={text}
                    alt="Topic"
                    width={64}
                    height={64}
                    style={{ objectFit: 'cover', borderRadius: '4px' }}
                    preview={false} // Disable preview for table view or enable if needed
                    fallback={'https://placehold.co/64x64/eee/ccc?text=Error'}
                />
            ) : <div style={{ width: 64, height: 64, display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#f0f0f0', borderRadius: '4px', color: '#aaa', fontSize: '12px' }}>No Img</div>,
        },
        {
            title: "Sort Num",
            dataIndex: "sortNumber",
            key: "sortNumber",
            width: 100,
            sorter: true,
            align: 'center',
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
            title: "Created Time",
            dataIndex: "createTime",
            key: "createTime",
            render: (text: string) => text ? new Date(text).toLocaleDateString() : '',
            sorter: true,
            width: 150,
        },
        {
            title: "Actions",
            key: "actions",
            width: 120,
            fixed: 'right',
            align: 'center',
            render: (_: any, record: API.TopicDto) => (
                <Space size="small">
                    <Typography.Link onClick={() => showEditModal(record)}>
                        Edit
                    </Typography.Link>
                    <Popconfirm
                        title="Delete Topic"
                        description="Are you sure you want to delete this topic?"
                        onConfirm={() => record.topicId && handleDeleteTopic(record.topicId)}
                        okText="Yes, Delete"
                        cancelText="Cancel"
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
            <Title level={3} className="mb-4 md:mb-6">Topic Management</Title>

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
                    <Col>
                        <Button
                            type="primary"
                            icon={<PlusOutlined />}
                            onClick={showAddModal}
                            className="shadow-sm"
                        >
                            Add Topic
                        </Button>
                    </Col>
                </Row>
            </Card>

            <Card bordered={false} className="shadow-sm">
                <Table
                    rowKey="topicId"
                    columns={columns}
                    dataSource={topics}
                    pagination={{
                        ...pagination,
                        showSizeChanger: true,
                        showTotal: (total, range) => `${range[0]}-${range[1]} of ${total} items`,
                    }}
                    loading={loading}
                    onChange={handleTableChange}
                    scroll={{ x: 1200 }}
                    className="admin-topic-table"
                />
            </Card>

            {modalVisible && (
                <TopicFormModal
                    visible={modalVisible}
                    mode={modalMode}
                    currentTopic={currentTopic}
                    onCancel={handleModalCancel}
                    onSuccess={handleModalSuccess}
                />
            )}
        </div>
    );
};

export default AdminTopicPage;
