"use client";
import { Button, Table, Space, Popconfirm, Typography, Input, Card, Row, Col, Tag } from "antd";
import { useEffect, useState } from "react";
import { PlusOutlined, SearchOutlined } from "@ant-design/icons";
import { message } from "antd";
import TopicFormModal from "@/components/admin/TopicFormModal";
import { getTopicColumns } from "@/app/admin/topic/topicColumns";
import { useTopicAdmin } from "@/app/admin/topic/useTopicAdmin";

const { Title } = Typography;

const AdminTopicPage = () => {
    // Get topic admin hooks
    const {
        handleDelete,
        fetchTopics,
    } = useTopicAdmin();

    // Table data state
    const [topics, setTopics] = useState<API.TopicDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [pagination, setPagination] = useState({
        current: 1,
        pageSize: 10,
        total: 0,
    });

    // Search state
    const [searchText, setSearchText] = useState('');

    // Modal states
    const [modalVisible, setModalVisible] = useState(false);
    const [modalMode, setModalMode] = useState<'add' | 'edit'>('add');
    const [currentTopic, setCurrentTopic] = useState<API.TopicDto | null>(null);

    // Fetch topics with pagination and search
    const fetchTopicsData = async (page = pagination.current, pageSize = pagination.pageSize, keyword = searchText) => {
        setLoading(true);
        try {
            const response = await fetchTopics(
                { current: page, pageSize, keyword: keyword || undefined },
                {},
                {}
            );

            if (response.success) {
                setTopics(response.data);
                setPagination({
                    ...pagination,
                    current: page,
                    pageSize,
                    total: response.total || 0,
                });
            }
        } catch (error) {
            console.error("Failed to fetch topics:", error);
            message.error("Failed to load topics");
        } finally {
            setLoading(false);
        }
    };

    // Fetch initial data
    useEffect(() => {
        fetchTopicsData();
    }, []);

    // Handle table pagination change
    const handleTableChange = (pagination: any) => {
        fetchTopicsData(pagination.current, pagination.pageSize);
    };

    // Handle search
    const handleSearch = () => {
        setPagination({ ...pagination, current: 1 });
        fetchTopicsData(1, pagination.pageSize, searchText);
    };

    // Open modal for adding new topic
    const showAddModal = () => {
        setCurrentTopic(null);
        setModalMode('add');
        setModalVisible(true);
    };

    // Open modal for editing a topic
    const showEditModal = (topic: API.TopicDto) => {
        setCurrentTopic(topic);
        setModalMode('edit');
        setModalVisible(true);
    };

    // Handle modal success (add or edit)
    const handleModalSuccess = () => {
        setModalVisible(false);
        fetchTopicsData();
    };

    return (
        <div className="p-6">
            <Title level={3}>Topic Management</Title>

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
                                Add Topic
                            </Button>
                        </Space>
                    </Col>
                </Row>

                <Table
                    rowKey="topicId"
                    columns={getTopicColumns(
                        showEditModal,
                        () => { }, // Not used
                        handleDelete
                    )}
                    dataSource={topics}
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

            <TopicFormModal
                visible={modalVisible}
                mode={modalMode}
                currentTopic={currentTopic}
                onCancel={() => setModalVisible(false)}
                onSuccess={handleModalSuccess}
            />
        </div>
    );
};

export default AdminTopicPage; 