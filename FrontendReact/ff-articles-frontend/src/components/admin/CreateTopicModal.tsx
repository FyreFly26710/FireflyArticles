import { apiTopicAddByRequest } from '@/api/contents/api/topic';
import { ProColumns, ProTable } from '@ant-design/pro-components';
import { message, Modal } from 'antd';
import React from 'react';

interface Props {
    visible: boolean;
    columns: ProColumns<API.TopicDto>[];
    onSubmit: (values: API.TopicAddRequest) => void;
    onCancel: () => void;
}

const handleAdd = async (fields: API.TopicAddRequest) => {
    const hide = message.loading('Adding');
    try {
        await apiTopicAddByRequest(fields);
        hide();
        message.success('Success');
        return true;
    } catch (error: any) {
        hide();
        message.error('Failed, ' + error.message);
        return false;
    }
};

const CreateTopicModal: React.FC<Props> = (props) => {
    const { visible, columns, onSubmit, onCancel } = props;

    return (
        <Modal
            destroyOnClose
            title={'Add Topic'}
            open={visible}
            footer={null}
            onCancel={() => {
                onCancel?.();
            }}
        >
            <ProTable
                type="form"
                columns={columns}
                onSubmit={async (values: API.TopicAddRequest) => {
                    const success = await handleAdd(values);
                    if (success) {
                        onSubmit?.(values);
                    }
                }}
            />
        </Modal>
    );
};
export default CreateTopicModal;
