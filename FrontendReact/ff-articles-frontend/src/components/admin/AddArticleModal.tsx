import { Button, Form, Input, InputNumber, Modal, Select, Space, Spin } from 'antd';
import { useEffect, useState } from 'react';
import TagSelect from '../shared/TagSelect';
import { apiArticleAddByRequest } from '@/api/contents/api/article';
import { apiTagGetAll } from '@/api/contents/api/tag';
import { apiTopicGetByPage, apiTopicGetById } from '@/api/contents/api/topic';
import { message } from 'antd';

const { Option } = Select;
const { TextArea } = Input;

interface AddArticleModalProps {
    visible: boolean;
    onCancel: () => void;
    onSuccess: () => void;
}

/**
 * Modal form for adding a new article
 */
const AddArticleModal = ({
    visible,
    onCancel,
    onSuccess
}: AddArticleModalProps) => {
    const [form] = Form.useForm();
    const [isParentDisabled, setIsParentDisabled] = useState(true);
    const [currentTags, setCurrentTags] = useState<string[]>([]);
    const [tags, setTags] = useState<API.TagDto[]>([]);
    const [topics, setTopics] = useState<API.TopicDto[]>([]);
    const [parentArticles, setParentArticles] = useState<API.ArticleDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [initializing, setInitializing] = useState(false);

    // Fetch data when modal opens
    useEffect(() => {
        if (visible) {
            fetchInitialData();
        }
    }, [visible]);

    // Fetch initial data (tags and topics)
    const fetchInitialData = async () => {
        setInitializing(true);
        try {
            // Fetch tags
            const tagsResponse = await apiTagGetAll();
            if (tagsResponse.data) {
                setTags(tagsResponse.data);
            }

            // Fetch topics
            const topicsResponse = await apiTopicGetByPage({
                PageSize: 100,
                PageNumber: 1
            });

            if (topicsResponse.data?.data) {
                setTopics(topicsResponse.data.data);
            }

            resetForm();
        } catch (error) {
            console.error('Error fetching initial data:', error);
            message.error('Failed to load initial data');
        } finally {
            setInitializing(false);
        }
    };

    // Fetch parent articles when topic is selected
    const handleTopicChange = async (topicId: number) => {
        if (!topicId) {
            setParentArticles([]);
            setIsParentDisabled(true);
            return;
        }

        setLoading(true);
        try {
            const response = await apiTopicGetById({
                id: topicId,
                IncludeArticles: true
            });

            if (response.data?.articles) {
                // Only include articles of type 'Article' as potential parents
                const filteredArticles = response.data.articles.filter(a => a.articleType === 'Article');
                setParentArticles(filteredArticles);

                // Update parent dropdown state based on article type
                const articleType = form.getFieldValue('articleType');
                setIsParentDisabled(articleType === 'Article' || filteredArticles.length === 0);
            } else {
                setParentArticles([]);
                setIsParentDisabled(true);
            }
        } catch (error) {
            console.error('Error fetching parent articles:', error);
            message.error('Failed to load parent articles');
            setParentArticles([]);
            setIsParentDisabled(true);
        } finally {
            setLoading(false);
        }
    };

    // Reset form with default values
    const resetForm = () => {
        form.setFieldsValue({
            articleType: 'Article',
            parentArticleId: 0,
            sortNumber: 0,
            isHidden: 0,
            tags: [],
            topicId: undefined
        });
        setCurrentTags([]);
        setParentArticles([]);
        setIsParentDisabled(true);
    };

    // Handle article type change
    const handleArticleTypeChange = (value: string) => {
        setIsParentDisabled(value === 'Article' || parentArticles.length === 0);

        if (value === 'Article') {
            form.setFieldsValue({ parentArticleId: 0 });
        } else {
            form.setFieldsValue({ parentArticleId: undefined });
        }
    };

    // Handle tag change
    const handleTagChange = (selectedTags: string[]) => {
        setCurrentTags(selectedTags);
        form.setFieldValue('tags', selectedTags);
    };

    // Handle form submission
    const handleSubmit = async (values: any) => {
        setLoading(true);
        try {
            // Transform form values to request format
            const requestData = {
                ...values,
                isHidden: values.isHidden ? 1 : 0
            };

            // Add new article
            const response = await apiArticleAddByRequest(requestData as API.ArticleAddRequest);
            const success = !!response.data;

            if (success) {
                message.success('Article added successfully');
                form.resetFields();
                onSuccess();
            } else {
                message.error(response.message || 'Failed to add article');
            }
        } catch (error: any) {
            message.error(`Operation failed: ${error.message}`);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            destroyOnClose={false}
            title="Add New Article"
            open={visible}
            footer={null}
            onCancel={onCancel}
            width={600}
        >
            <Spin spinning={initializing || loading} tip={initializing ? "Loading data..." : loading ? "Processing..." : undefined}>
                <Form
                    form={form}
                    layout="horizontal"
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 18 }}
                    onFinish={handleSubmit}
                >
                    <Form.Item
                        label="Topic"
                        name="topicId"
                        rules={[{ required: true, message: 'Topic is required' }]}
                    >
                        <Select
                            placeholder="Select a topic"
                            onChange={handleTopicChange}
                            loading={initializing}
                            disabled={initializing}
                        >
                            {topics.map(topic => (
                                <Option key={topic.topicId} value={topic.topicId}>
                                    {topic.title}
                                </Option>
                            ))}
                        </Select>
                    </Form.Item>

                    <Form.Item
                        label="Title"
                        name="title"
                        rules={[{ required: true, message: 'Title is required' }]}
                    >
                        <Input />
                    </Form.Item>

                    <Form.Item label="Abstract" name="abstract">
                        <TextArea rows={2} />
                    </Form.Item>

                    <Form.Item label="Content" name="content">
                        <TextArea rows={3} />
                    </Form.Item>

                    <Form.Item
                        label="Tags"
                        name="tags"
                    >
                        <TagSelect
                            tags={tags}
                            selectedTags={currentTags}
                            onChange={handleTagChange}
                        />
                    </Form.Item>

                    <Form.Item label="Article Type" name="articleType">
                        <Select
                            onChange={handleArticleTypeChange}
                            disabled={parentArticles.length === 0}
                        >
                            <Option value="Article">Article</Option>
                            <Option value="SubArticle">SubArticle</Option>
                        </Select>
                    </Form.Item>

                    <Form.Item
                        label="Parent Article"
                        name="parentArticleId"
                        rules={[{ required: true, message: 'Parent Article is required' }]}
                    >
                        <Select
                            allowClear
                            disabled={isParentDisabled}
                            loading={loading && !initializing}
                            placeholder={parentArticles.length ? "Select parent article" : "No parent articles available"}
                        >
                            {parentArticles.map((article) => (
                                <Option key={article.articleId} value={article.articleId}>
                                    {article.title}
                                </Option>
                            ))}
                        </Select>
                    </Form.Item>

                    <Form.Item label="Sort Number" name="sortNumber">
                        <InputNumber min={0} />
                    </Form.Item>

                    <Form.Item label="Hidden" name="isHidden">
                        <Select>
                            <Option value={0}>Visible</Option>
                            <Option value={1}>Hidden</Option>
                        </Select>
                    </Form.Item>

                    <Form.Item wrapperCol={{ span: 24 }}>
                        <div className="flex justify-between">
                            <Button
                                onClick={resetForm}
                                disabled={initializing || loading}
                            >
                                Reset
                            </Button>
                            <Space>
                                <Button
                                    onClick={onCancel}
                                    disabled={loading}
                                >
                                    Cancel
                                </Button>
                                <Button
                                    type="primary"
                                    htmlType="submit"
                                    loading={loading && !initializing}
                                    disabled={initializing}
                                >
                                    Add
                                </Button>
                            </Space>
                        </div>
                    </Form.Item>
                </Form>
            </Spin>
        </Modal>
    );
};

export default AddArticleModal; 