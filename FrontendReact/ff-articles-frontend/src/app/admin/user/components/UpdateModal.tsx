import { postAdminUpdate } from '@/api/identity/api/admin';
import { ProColumns, ProTable } from '@ant-design/pro-components';
import { message, Modal } from 'antd';
import React from 'react';

interface Props {
    oldData?: API.UserResponse; 
    visible: boolean; 
    columns: ProColumns<API.UserResponse>[]; 
    onSubmit: (values: API.UserUpdateRequest) => void; 
    onCancel: () => void; 
}

/**
 * Handle the update operation
 *
 * @param fields - The form values submitted by the user
 */
const handleUpdate = async (fields: API.UserUpdateRequest) => {
    const hide = message.loading('Updating...');
    try {
        await postAdminUpdate(fields);
        hide();
        message.success('Update successful');
        return true;
    } catch (error: any) {
        hide();
        message.error('Update failed, ' + error.message);
        return false;
    }
};

/**
 * Update Modal Component
 *
 * @param props - Props passed to the component
 * @constructor
 */
const UpdateModal: React.FC<Props> = (props) => {
    const { oldData, visible, columns, onSubmit, onCancel } = props;

    // If no data is provided, render nothing
    if (!oldData) {
        return <></>;
    }

    return (
        <Modal
            destroyOnClose
            title={'Update'} 
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
                onSubmit={async (values: API.UserUpdateRequest) => {
                    // Handle form submission
                    const success = await handleUpdate({
                        ...values,
                        id: oldData?.id as any, 
                    } as any);
                    if (success) {
                        onSubmit?.(values); 
                    }
                }}
            />
        </Modal>
    );
};
export default UpdateModal;
