import { Button, Form, Input, InputNumber, Modal, Select, Space, Spin } from 'antd';
import { useEffect, useState } from 'react';
import TagSelect from './TagSelect';
import { useArticleModal } from '@/hooks/useArticleModal';

const { Option } = Select;
const { TextArea } = Input;

/**
 * Modal form for editing article properties
 */
const ArticleFormModal = () => {
    const {
        isVisible,
        currentArticle,
        tags,
        parentArticles,
        loading,
        closeModal,
        submitArticleEdit
    } = useArticleModal();

    const [form] = Form.useForm();
    const [isParentDisabled, setIsParentDisabled] = useState(false);
    const [currentTags, setCurrentTags] = useState<string[]>([]);

    // Initialize form when modal opens or data changes
    useEffect(() => {
        if (isVisible && currentArticle) {
            resetForm();
        }
    }, [isVisible, currentArticle, parentArticles]);

    // Reset form to current article values
    const resetForm = () => {
        if (!currentArticle) return;

        const formValues = {
            ...currentArticle,
            parentArticleId: currentArticle.parentArticleId || 0,
            topicId: currentArticle.topicId || undefined,
            isHidden: currentArticle.isHidden === 1,
            tags: currentArticle.tags || [],
        };

        form.setFieldsValue(formValues);
        setCurrentTags(formValues.tags);

        // Set parent disabled state based on article type and available parents
        const shouldDisableParent = currentArticle.articleType === 'Article' || parentArticles.length === 0;
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
    const handleSubmit = async (values: API.ArticleEditRequest) => {
        console.log('Form submitted with tags:', values.tags);
        await submitArticleEdit(values);
    };

    return (
        <Modal
            destroyOnClose={false}
            title="Edit Article"
            open={isVisible}
            footer={null}
            onCancel={closeModal}
            width={600}
        >
            <Spin spinning={loading}>
                <Form
                    form={form}
                    layout="horizontal"
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 18 }}
                    onFinish={handleSubmit}
                    onValuesChange={(changedValues) => {
                        if ('tags' in changedValues) {
                            console.log('Form values changed for tags:', changedValues.tags);
                        }
                    }}
                >
                    <Form.Item label="Article ID" name="articleId">
                        <Input disabled />
                    </Form.Item>

                    <Form.Item label="Topic" name="topicTitle">
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

                    <Form.Item wrapperCol={{ span: 24 }}>
                        <div className="flex justify-between">
                            <Button onClick={resetForm}>
                                Reset
                            </Button>
                            <Space>
                                <Button onClick={closeModal}>
                                    Cancel
                                </Button>
                                <Button type="primary" htmlType="submit">
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