'use client';

import { Button, Form, Input, InputNumber, Modal, Select, Space, Spin, Upload } from 'antd';
import { useEffect, useState } from 'react';
import { apiTopicAddByRequest, apiTopicEditByRequest, apiTopicGetByPage } from '@/api/contents/api/topic';
import { message } from 'antd';
import { getTopicsByCategory } from '@/libs/utils/articleUtils';

const { Option } = Select;
const { TextArea } = Input;

interface TopicFormModalProps {
    visible: boolean;
    mode: 'add' | 'edit';
    currentTopic?: API.TopicDto | null;
    onCancel: () => void;
    onSuccess: () => void;
}

/**
 * Modal form for adding or editing topic properties
 */
const TopicFormModal = ({
    visible,
    mode,
    currentTopic,
    onCancel,
    onSuccess
}: TopicFormModalProps) => {
    const [form] = Form.useForm();
    const [loading, setLoading] = useState(false);
    const [topicsByCategory, setTopicsByCategory] = useState<Record<string, API.TopicDto[]>>({});

    const isEdit = mode === 'edit';
    const title = isEdit ? "Edit Topic" : "Add New Topic";

    // Initialize form when modal opens or data changes
    useEffect(() => {
        const fetchTopics = async () => {
            const response = await apiTopicGetByPage({
                PageNumber: 1,
                PageSize: 100,
                OnlyCategoryTopic: true,
            });

            const topicsData = response.data?.data || [];

            const topicsByCategory = getTopicsByCategory(topicsData);
            setTopicsByCategory(topicsByCategory);
        }

        if (visible) {
            fetchTopics();
            if (isEdit && currentTopic) {
                resetForm(currentTopic);
            } else {
                // Default values for new topic
                form.setFieldsValue({
                    category: 'Normal',
                    sortNumber: 0,
                    isHidden: 0,
                });
            }
        }
    }, [visible, currentTopic, isEdit]);

    // Reset form to topic values
    const resetForm = (topic: API.TopicDto) => {
        const formValues = {
            ...topic,
            isHidden: topic.isHidden,
        };

        form.setFieldsValue(formValues);
    };

    // Handle form submission
    const handleSubmit = async (values: any) => {
        setLoading(true);
        try {
            // Transform form values to request format
            const requestData = {
                ...values,
            };

            let success = false;

            if (isEdit) {
                // Edit existing topic
                const response = await apiTopicEditByRequest(requestData as API.TopicEditRequest);
                success = !!response.data;
                if (success) {
                    message.success('Topic updated successfully');
                } else {
                    message.error(response.message || 'Failed to update topic');
                }
            } else {
                // Add new topic
                const response = await apiTopicAddByRequest(requestData as API.TopicAddRequest);
                success = !!response.data;
                if (success) {
                    message.success('Topic added successfully');
                } else {
                    message.error(response.message || 'Failed to add topic');
                }
            }

            if (success) {
                form.resetFields();
                onSuccess();
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
            title={title}
            open={visible}
            footer={null}
            onCancel={onCancel}
            width={600}
        >
            <Spin spinning={loading}>
                <Form
                    form={form}
                    layout="horizontal"
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 18 }}
                    onFinish={handleSubmit}
                >
                    {isEdit && (
                        <Form.Item label="Topic ID" name="topicId">
                            <Input disabled />
                        </Form.Item>
                    )}

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

                    <Form.Item label="Topic Image" name="topicImage">
                        <Input placeholder="Enter image URL" />
                    </Form.Item>

                    <Form.Item label="Category" name="category">
                        <Select>
                            {Object.keys(topicsByCategory).map((category) => (
                                <Option key={category} value={category}>
                                    {category}
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
                            <Button onClick={() => isEdit && currentTopic ? resetForm(currentTopic) : form.resetFields()}>
                                Reset
                            </Button>
                            <Space>
                                <Button onClick={onCancel}>
                                    Cancel
                                </Button>
                                <Button type="primary" htmlType="submit">
                                    {isEdit ? 'Update' : 'Add'}
                                </Button>
                            </Space>
                        </div>
                    </Form.Item>
                </Form>
            </Spin>
        </Modal>
    );
};

export default TopicFormModal; 