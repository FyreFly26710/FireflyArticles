import { apiArticleEditByRequest } from '@/api/contents/api/article';
import { Button, Form, Input, InputNumber, message, Modal, Select, Tag } from 'antd';
import React, { useEffect, useState } from 'react';
import TagSelect from '../TagSelect';

const { Option } = Select;

interface Props {
    parentArticleList: API.ArticleMiniDto[] | [] | undefined;
    tagList: API.TagDto[] | [];
    currentArticle: API.ArticleDto;
    modifyContent:boolean;
    visible: boolean;
    onSubmit: (values: API.ArticleEditRequest) => void;
    onCancel: () => void;
}

const ArticleFormModal: React.FC<Props> = (props) => {
    const { parentArticleList, currentArticle, tagList, visible, onSubmit, onCancel, modifyContent=true } = props;
    const [form] = Form.useForm();
    const [isParentDisabled, setIsParentDisabled] = useState(false);
    const filterParentArticleList = parentArticleList?.filter(a => a.articleId !== currentArticle.articleId);

    useEffect(() => {
        if (visible && currentArticle) {
            resetForm(currentArticle);
        }
    }, [visible, currentArticle]);


    const resetForm = (article: API.ArticleDto | undefined) => {
        if (article) {
            const initialTagIds = (article.tags || [])
                .map(tagName => tagList.find(tag => tag.tagName === tagName)?.tagId)
                .filter((id): id is number => id !== undefined);
            form.setFieldsValue({
                ...article,
                parentArticleId: article.parentArticleId || 0,
                topicId: article.topicId || undefined,
                isHidden: article.isHidden === 1,
                tagIds: initialTagIds,
            });
            setIsParentDisabled(article.articleType === 'Article' || filterParentArticleList?.length === 0);
        }
        else {
            form.resetFields();
            setIsParentDisabled(true);
        }
    }
    const handleArticleTypeChange = (value: string) => {
        setIsParentDisabled(value === 'Article' || filterParentArticleList?.length === 0);
        if (value === 'Article') {
            form.setFieldsValue({ parentArticleId: 0 });
        } else {
            form.setFieldsValue({ parentArticleId: undefined });
        }
    };

    const handleFinish = async (values: API.ArticleEditRequest) => {
        const hide = message.loading('Updating...');
        try {
            await apiArticleEditByRequest({ 
                ...values,
                content:modifyContent? values.content : undefined, 
                isHidden: values.isHidden ? 1 : 0 });
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
                {/* TODO: Add topic selection */}
                <Form.Item label="Topic" name="topicTitle">
                    {/* <Select disabled value={true}>
                        {Array.isArray(topicList) &&
                            topicList.map((topic) => (
                                <Option key={topic.topicId} value={topic.topicId}>
                                    {topic.title}
                                </Option>
                            ))}
                    </Select> */}
                    <Input disabled />
                </Form.Item>

                <Form.Item label='Title' name='title' rules={[{ required: true, message: 'Title is required' }]} >
                    <Input />
                </Form.Item>
                <Form.Item label='Abstraction' name='abstraction'>
                    <Input.TextArea rows={1} />
                </Form.Item>
                <Form.Item label='Content' name='content'>
                    <Input.TextArea disabled={!modifyContent} rows={2} />
                </Form.Item>
                <Form.Item label="Tags" name="tagIds">
                    <Select
                        mode="multiple"
                        style={{ width: '100%' }}
                        dropdownStyle={{
                            padding: 8,
                            maxWidth: '40vw'
                        }}
                        popupMatchSelectWidth={true}
                        dropdownAlign={{
                            points: ['tl', 'bl'],
                            offset: [0, 4],
                            overflow: {
                                adjustX: true,
                                adjustY: true
                            }
                        }}
                        dropdownRender={() => (
                            <TagSelect
                                tagData={tagList}
                                selectedTagIds={form.getFieldValue('tagIds') || []}
                                onSelectionChange={(selectedIds) => {
                                    form.setFieldValue('tagIds', selectedIds);
                                }}
                            />
                        )}
                        value={form.getFieldValue('tagIds') || []}
                        onChange={(value) => form.setFieldValue('tagIds', value)}
                    >
                        {tagList.map(tag => (
                            <Option key={tag.tagId} value={tag.tagId}>
                                {tag.tagName}
                            </Option>
                        ))}
                    </Select>
                </Form.Item>

                <Form.Item label='Article Type' name='articleType'>
                    <Select onChange={handleArticleTypeChange} disabled={filterParentArticleList?.length === 0}>
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