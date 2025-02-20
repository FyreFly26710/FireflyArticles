
import { apiArticleAddByRequest } from '@/api/contents/api/article';
import { ProColumns, ProTable } from '@ant-design/pro-components';
import { message, Modal } from 'antd';
import React from 'react';

interface Props {
    visible: boolean;
    columns: ProColumns<API.ArticleDto>[];
    onSubmit: (values: API.ArticleAddRequest) => void;
    onCancel: () => void;
}

const handleAdd = async (fields: API.ArticleAddRequest) => {
    const hide = message.loading('Adding');
    try {
        await apiArticleAddByRequest(fields);
        hide();
        message.success('Success');
        return true;
    } catch (error: any) {
        hide();
        message.error('Failed, ' + error.message);
        return false;
    }
};

const CreateModal: React.FC<Props> = (props) => {
    const { visible, columns, onSubmit, onCancel } = props;

    return (
        <Modal
            destroyOnClose
            title={'Add Article'}
            open={visible}
            footer={null}
            onCancel={() => {
                onCancel?.();
            }}
        >
            <ProTable
                type="form"
                columns={columns}
                onSubmit={async (values: API.ArticleAddRequest) => {
                    const success = await handleAdd(values);
                    if (success) {
                        onSubmit?.(values);
                    }
                }}
            />
        </Modal>
    );
};
export default CreateModal;
