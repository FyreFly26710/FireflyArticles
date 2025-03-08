import React, { useState } from 'react';
import { Tag, Input, Tooltip, message, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { apiTagAddByRequest, apiTagEditByRequest, apiTagDeleteById } from '@/api/contents/api/tag';

interface TagEditProps {
    tags: API.TagDto[];
    enableEdit?: boolean;
    onTagsChange?: () => void;
}

const TagEdit: React.FC<TagEditProps> = ({ tags, enableEdit = false, onTagsChange }) => {
    const [inputVisible, setInputVisible] = useState(false);
    const [inputValue, setInputValue] = useState('');
    const [editingTagId, setEditingTagId] = useState<number | null>(null);
    const [editInputValue, setEditInputValue] = useState('');

    const handleClose = async (tagId: number | undefined) => {
        if (!tagId) return;
        try {
            await apiTagDeleteById({ id: tagId });
            message.success('Tag deleted successfully');
            onTagsChange?.();
        } catch (error: any) {
            message.error('Failed to delete tag: ' + error.message);
        }
    };

    const handleInputConfirm = async () => {
        if (!inputValue) {
            setInputVisible(false);
            return;
        }

        try {
            await apiTagAddByRequest({ tagName: inputValue });
            message.success('Tag added successfully');
            setInputValue('');
            setInputVisible(false);
            onTagsChange?.();
        } catch (error: any) {
            message.error('Failed to add tag: ' + error.message);
        }
    };

    const handleEditInputConfirm = async (tagId: number) => {
        if (!editInputValue) {
            setEditingTagId(null);
            return;
        }

        try {
            await apiTagEditByRequest({ tagId, tagName: editInputValue });
            message.success('Tag updated successfully');
            setEditingTagId(null);
            setEditInputValue('');
            onTagsChange?.();
        } catch (error: any) {
            message.error('Failed to update tag: ' + error.message);
        }
    };

    return (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '10px' }}>
            {tags.map(tag => {
                if (enableEdit && editingTagId === tag.tagId) {
                    return (
                        <Input
                            key={tag.tagId}
                            size="small"
                            style={{ width: 110 }}
                            value={editInputValue}
                            onChange={e => setEditInputValue(e.target.value)}
                            onBlur={() => handleEditInputConfirm(tag.tagId!)}
                            onPressEnter={() => handleEditInputConfirm(tag.tagId!)}
                        />
                    );
                }

                const tagElement = (
                    <Tag
                        key={tag.tagId}
                        closable={enableEdit}
                        onClose={e => {
                            e.preventDefault();
                            handleClose(tag.tagId);
                        }}
                    >
                        <span>{tag.tagName}</span>
                        {enableEdit && (
                            <EditOutlined
                                style={{ marginLeft: 8 }}
                                onClick={() => {
                                    setEditingTagId(tag.tagId!);
                                    setEditInputValue(tag.tagName!);
                                }}
                            />
                        )}
                    </Tag>
                );

                return (
                    tagElement
                );
            })}

            {enableEdit && (
                <>
                    {inputVisible ? (
                        <Input
                            type="text"
                            size="small"
                            style={{ width: 78 }}
                            value={inputValue}
                            onChange={e => setInputValue(e.target.value)}
                            onBlur={handleInputConfirm}
                            onPressEnter={handleInputConfirm}
                        />
                    ) : (
                        <Tag onClick={() => setInputVisible(true)} className="site-tag-plus">
                            <PlusOutlined /> New Tag
                        </Tag>
                    )}
                </>
            )}
        </div>
    );
};

export default TagEdit;
