
import { apiArticleEditByRequest } from '@/api/contents/api/article';
import { ProColumns, ProTable } from '@ant-design/pro-components';
import { message, Modal } from 'antd';
import React from 'react';

interface Props {
    oldData?: API.ArticleDto;
    visible: boolean;
    columns: ProColumns<API.ArticleDto>[];
    onSubmit: (values: API.ArticleEditRequest) => void;
    onCancel: () => void;
}

const handleUpdate = async (fields: API.ArticleEditRequest) => {
    const hide = message.loading('Adding');
    try {
        await apiArticleEditByRequest(fields);
        hide();
        message.success('Successful');
        return true;
    } catch (error: any) {
        hide();
        message.error('Failed, ' + error.message);
        return false;
    }
};

const UpdateModal: React.FC<Props> = (props) => {
    const { oldData, visible, columns, onSubmit, onCancel } = props;

    if (!oldData) {
        return <></>;
    }

    return (
        <Modal
            destroyOnClose
            title={'Update Article'}
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
                onSubmit={async (values: API.ArticleEditRequest) => {
                    const success = await handleUpdate({
                        ...values,
                        articleId: oldData.articleId as any,
                    });
                    if (success) {
                        onSubmit?.(values);
                    }
                }}
            />
        </Modal>
    );
};
export default UpdateModal;