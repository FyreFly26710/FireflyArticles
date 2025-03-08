import React, { useEffect } from 'react';
import { Modal } from 'antd';
import TagEdit from '@/components/TagEdit';

interface TagModalProps {
    visible: boolean;
    tags: API.TagDto[];
    onCancel: () => void;
    onTagsChange: () => void;
}

const TagModal: React.FC<TagModalProps> = ({ visible, tags, onCancel, onTagsChange }) => {

    return (
        <Modal
            title="Manage Tags"
            open={visible}
            onCancel={onCancel}
            footer={null}
        >
            <TagEdit
                tags={tags}
                enableEdit={true}
                onTagsChange={onTagsChange}
            />
        </Modal>
    );
};

export default TagModal;
