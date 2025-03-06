import { apiArticleEditByRequest } from '@/api/contents/api/article';
import { Button, Form, Input, InputNumber, message, Modal, Select } from 'antd';
import React, { useEffect, useState } from 'react';

const { Option } = Select;

interface Props {
    parentArticleList: API.ArticleMiniDto[] | [] | undefined;
    topicList: API.TopicDto[] | [];
    tagList: API.TagDto[] | [];
    currentArticle: API.ArticleDto;
    visible: boolean;
    onSubmit: (values: API.ArticleEditRequest) => void;
    onCancel: () => void;
}

const ArticleFormModal: React.FC<Props> = (props) => {
    const { parentArticleList, topicList, currentArticle, tagList, visible, onSubmit, onCancel } = props;
    const [form] = Form.useForm();
    const [isParentDisabled, setIsParentDisabled] = useState(false);
    const filterParentArticleList = parentArticleList?.filter(a => a.articleId !== currentArticle.articleId);

    useEffect(() => {
        resetForm(currentArticle);
    }, []);


    const resetForm = (article: API.ArticleDto | undefined) => {
        if (article) {
            const initialTagIds = (article.tags || [])
                .map(tagName => tagList.find(tag => tag.tagName === tagName)?.tagId)
                .filter((id): id is number => id !== undefined);
            form.setFieldsValue({
                ...article,
                parentArticleId: article.parentArticleId || undefined,
                topicId: article.topicId || undefined,
                isHidden: article.isHidden === 1,
                tagIds: initialTagIds,
            });
            setIsParentDisabled(article.articleType === 'Article');
        }
        else {
            form.resetFields();
            setIsParentDisabled(true);
        }
    }



    const handleArticleTypeChange = (value: string) => {
        setIsParentDisabled(value === 'Article');
        if (value === 'Article') {
            form.setFieldsValue({ parentArticleId: 0 });
        } else {
            form.setFieldsValue({ parentArticleId: undefined });
        }
    };

    const handleFinish = async (values: API.ArticleEditRequest) => {
        const hide = message.loading('Updating...');
        try {
            await apiArticleEditByRequest({ ...values, isHidden: values.isHidden ? 1 : 0 });
            hide();
            message.success('Update successful!');
            onSubmit(values);
        } catch (error: any) {
            hide();
            message.error('Update failed: ' + error.message);
        }
    };

    return (
        <Modal
            destroyOnClose
            title='Update Article'
            open={visible}
            footer={null}
            onCancel={onCancel}
        >
            <Form form={form}
                layout='horizontal'
                labelCol={{ span: 6 }}
                wrapperCol={{ span: 18 }}
                onFinish={handleFinish}
            >
                <Form.Item label='Article ID' name='articleId'>
                    <Input disabled />
                </Form.Item>
                <Form.Item label="Topic" name="topicId">
                    <Select disabled value={currentArticle.topicId}>
                        {Array.isArray(topicList) &&
                            topicList.map((topic) => (
                                <Option key={topic.topicId} value={topic.topicId}>
                                    {topic.title}
                                </Option>
                            ))}
                    </Select>
                </Form.Item>

                <Form.Item label='Title' name='title' rules={[{ required: true, message: 'Title is required' }]} >
                    <Input />
                </Form.Item>
                <Form.Item label='Abstraction' name='abstraction'>
                    <Input.TextArea rows={1} />
                </Form.Item>
                <Form.Item label='Content' name='content'>
                    <Input.TextArea rows={2} />
                </Form.Item>
                <Form.Item label="Tags" name="tagIds">
                    <Select
                        mode="multiple"
                        optionFilterProp="children"
                        showSearch
                        filterOption={(input, option) =>
                            String(option?.children).toLowerCase().includes(input.toLowerCase())
                        }
                    >
                        {tagList.map(tag => (
                            <Option key={tag.tagId} value={tag.tagId}>
                                {tag.tagName}
                            </Option>
                        ))}
                    </Select>
                </Form.Item>

                <Form.Item label='Article Type' name='articleType'>
                    <Select onChange={handleArticleTypeChange}>
                        <Option value='Article'>Article</Option>
                        <Option value='SubArticle'>SubArticle</Option>
                    </Select>
                </Form.Item>
                <Form.Item label='Parent Article' name='parentArticleId' rules={[{ required: true, message: 'Parent Article is required' }]}>
                    <Select allowClear disabled={isParentDisabled}>
                        {filterParentArticleList?.map((article) => (
                            <Option key={article.articleId} value={article.articleId}>
                                {article.title}
                            </Option>
                        ))}
                    </Select>
                </Form.Item>
                <Form.Item label='Sort Number' name='sortNumber'>
                    <InputNumber min={0} />
                </Form.Item>
                <Form.Item wrapperCol={{ span: 24 }} style={{ textAlign: 'right' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', gap: 20 }}>
                        <Button onClick={() => resetForm(currentArticle)}>Reset</Button>
                        <div>
                            <Button style={{ marginLeft: 20 }} onClick={() => {
                                onCancel();
                                resetForm(currentArticle);
                            }}>Cancel</Button>
                            <Button style={{ marginLeft: 20 }} type='primary' htmlType='submit'>Update</Button>
                        </div>
                    </div>
                </Form.Item>


            </Form>
        </Modal >
    );
};

export default ArticleFormModal;