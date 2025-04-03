
import { apiTopicEditByRequest } from '@/api/contents/api/topic';
import { ProColumns, ProTable } from '@ant-design/pro-components';
import { message, Modal } from 'antd';
import React from 'react';

interface Props {
    oldData?: API.TopicDto;
    visible: boolean;
    columns: ProColumns<API.TopicDto>[];
    onSubmit: (values: API.TopicEditRequest) => void;
    onCancel: () => void;
}

const handleUpdate = async (fields: API.TopicEditRequest) => {
    const hide = message.loading('Adding');
    try {
        await apiTopicEditByRequest(fields);
        hide();
        message.success('Successful');
        return true;
    } catch (error: any) {
        hide();
        message.error('Failed, ' + error.message);
        return false;
    }
};

const UpdateTopicModal: React.FC<Props> = (props) => {
    const { oldData, visible, columns, onSubmit, onCancel } = props;

    if (!oldData) {
        return <></>;
    }

    return (
        <Modal
            destroyOnClose
            title={'Update Topic'}
            open={visible}
            footer={null}
            onCancel={() => {
                onCancel?.();
            }}
        >
            <ProTable
                type="form"
                columns={columns}
                form={{
                    initialValues: oldData,
                }}
                onSubmit={async (values: API.TopicEditRequest) => {
                    const success = await handleUpdate({
                        ...values,
                        topicId: oldData.topicId as any,
                    });
                    if (success) {
                        onSubmit?.(values);
                    }
                }}
            />
        </Modal>
    );
};
export default UpdateTopicModal;