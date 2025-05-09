import { Button, Form, Input, InputNumber, Modal, Select, Space, Spin } from 'antd';
import { useEffect, useState } from 'react';
import TagSelect from './TagSelect';
import { apiArticleEditByRequest } from '@/api/contents/api/article';
import { apiTagGetAll } from '@/api/contents/api/tag';
import { apiTopicGetById } from '@/api/contents/api/topic';
import { message } from 'antd';

const { Option } = Select;
const { TextArea } = Input;

interface ArticleFormModalProps {
    visible: boolean;
    currentArticle?: API.ArticleDto | null;
    onCancel: () => void;
    onSuccess: () => void;
}

/**
 * Modal form for editing article properties
 */
const ArticleFormModal = ({
    visible,
    currentArticle,
    onCancel,
    onSuccess
}: ArticleFormModalProps) => {
    const [form] = Form.useForm();
    const [isParentDisabled, setIsParentDisabled] = useState(false);
    const [currentTags, setCurrentTags] = useState<string[]>([]);
    const [tags, setTags] = useState<API.TagDto[]>([]);
    const [parentArticles, setParentArticles] = useState<API.ArticleDto[]>([]);
    const [topic, setTopic] = useState<API.TopicDto | null>(null);
    const [loading, setLoading] = useState(false);
    const [initializing, setInitializing] = useState(false);

    // Fetch data and initialize form when modal opens
    useEffect(() => {
        if (visible && currentArticle) {
            fetchDataAndInitialize(currentArticle);
        }
    }, [visible, currentArticle]);

    // Fetch all necessary data and initialize the form
    const fetchDataAndInitialize = async (article: API.ArticleDto) => {
        setInitializing(true);
        try {
            // Fetch tags
            const tagsResponse = await apiTagGetAll();
            if (tagsResponse.data) {
                setTags(tagsResponse.data);
            }

            // Fetch topic with articles to get potential parent articles
            if (article.topicId) {
                const topicResponse = await apiTopicGetById({
                    id: article.topicId,
                    IncludeArticles: true,
                });

                if (topicResponse.data?.articles) {
                    // Filter out the current article from potential parents
                    // Only include articles of type 'Article' as parents
                    const filteredArticles = topicResponse.data.articles.filter(
                        a => a.articleId !== article.articleId && a.articleType === 'Article'
                    );
                    setParentArticles(filteredArticles);
                    setTopic(topicResponse.data);
                }
            }

            // Initialize form with article data
            resetForm(article);
        } catch (error) {
            console.error('Error fetching data:', error);
            message.error('Failed to load necessary data');
        } finally {
            setInitializing(false);
        }
    };

    // Reset form to article values
    const resetForm = (article: API.ArticleDto) => {
        const formValues = {
            ...article,
            parentArticleId: article.parentArticleId || 0,
            topicId: article.topicId || undefined,
            isHidden: article.isHidden === 1,
            tags: article.tags || [],
            topicTitle: topic?.title || article.topicTitle || ''
        };

        form.setFieldsValue(formValues);
        setCurrentTags(formValues.tags);

        // Set parent disabled state based on article type and available parents
        const shouldDisableParent = article.articleType === 'Article' || parentArticles.length === 0;
        setIsParentDisabled(shouldDisableParent);
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

            // Edit existing article
            const response = await apiArticleEditByRequest(requestData as API.ArticleEditRequest);
            const success = !!response.data;

            if (success) {
                message.success('Article updated successfully');
                form.resetFields();
                onSuccess();
            } else {
                message.error(response.message || 'Failed to update article');
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
            title="Edit Article"
            open={visible}
            footer={null}
            onCancel={onCancel}
            width={600}
        >
            <Spin spinning={initializing || loading} tip={initializing ? "Loading data..." : "Saving..."}>
                <Form
                    form={form}
                    layout="horizontal"
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 18 }}
                    onFinish={handleSubmit}
                >
                    <Form.Item label="Topic" name="topicTitle">
                        <Input disabled />
                    </Form.Item>

                    <Form.Item label="Article ID" name="articleId">
                        <Input disabled />
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
                        <Select allowClear disabled={isParentDisabled}>
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

                    {/* <Form.Item label="Hidden" name="isHidden" valuePropName="checked">
                        <Select>
                            <Option value={0}>Visible</Option>
                            <Option value={1}>Hidden</Option>
                        </Select>
                    </Form.Item> */}

                    <Form.Item wrapperCol={{ span: 24 }}>
                        <div className="flex justify-between">
                            <Button
                                onClick={() => currentArticle ? resetForm(currentArticle) : form.resetFields()}
                                disabled={initializing}
                            >
                                Reset
                            </Button>
                            <Space>
                                <Button onClick={onCancel} disabled={loading}>
                                    Cancel
                                </Button>
                                <Button
                                    type="primary"
                                    htmlType="submit"
                                    loading={loading}
                                    disabled={initializing}
                                >
                                    Update
                                </Button>
                            </Space>
                        </div>
                    </Form.Item>
                </Form>
            </Spin>
        </Modal>
    );
};

export default ArticleFormModal;