"use client";
import { postAdminList, postAdminOpenApiDelete } from '@/api/identity/api/admin';
import { PlusOutlined } from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { Button, message, Space, Typography } from 'antd';
import React, { useRef, useState } from 'react';
import UpdateModal from './components/UpdateModal';

const UserAdminPage: React.FC = () => {
    const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
    const actionRef = useRef<ActionType>();
    const [currentRow, setCurrentRow] = useState<API.UserResponse>();


    const handleDelete = async (row: API.UserResponse) => {
        const hide = message.loading('Deleting');
        if (!row) return true;
        try {
            await postAdminOpenApiDelete({
                id: row.id as any,
            });
            hide();
            message.success('Deleted');
            actionRef?.current?.reload();
            return true;
        } catch (error: any) {
            hide();
            message.error('Failed deleting user, ' + error.message);
            return false;
        }
    };


    const columns: ProColumns<API.UserResponse>[] = [
        {
            title: 'id',
            dataIndex: 'id',
            valueType: 'text',
            hideInForm: true,
        },
        {
            title: 'Account',
            dataIndex: 'userAccount',
            valueType: 'text',
            hideInForm: true,
        },
        {
            title: 'User Name',
            dataIndex: 'userName',
            valueType: 'text',
        },
        {
            title: 'User Email',
            dataIndex: 'userEmail',
            valueType: 'text',
        },
        {
            title: 'Avatar',
            dataIndex: 'userAvatar',
            valueType: 'image',
            fieldProps: {
                width: 64,
            },
            hideInSearch: true,
        },
        {
            title: 'Profile',
            dataIndex: 'userProfile',
            valueType: 'textarea',
        },
        {
            title: 'User Role',
            dataIndex: 'userRole',
            valueEnum: {
                user: {
                    text: 'User',
                },
                admin: {
                    text: 'Admin',
                },
            },
        },
        {
            title: 'Created Date',
            sorter: true,
            dataIndex: 'createTime',
            valueType: 'dateTime',
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: 'Updated Date',
            sorter: true,
            dataIndex: 'updateTime',
            valueType: 'dateTime',
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: 'Options',
            dataIndex: 'option',
            valueType: 'option',
            render: (_, record) => (
                <Space size="middle">
                    <Typography.Link
                        onClick={() => {
                            setCurrentRow(record);
                            setUpdateModalVisible(true);
                        }}
                    >
                        Edit
                    </Typography.Link>
                    <Typography.Link type="danger" onClick={() => handleDelete(record)}>
                        Delete
                    </Typography.Link>
                </Space>
            ),
        },
    ];
    return (
        <PageContainer>
            <ProTable<API.UserResponse>
                headerTitle={'User Table'}
                actionRef={actionRef}
                rowKey="key"
                search={{
                    labelWidth: 120,
                }}
                toolBarRender={() => [
                    <Button
                        type="primary"
                        key="primary"
                        onClick={() => {
                            setCreateModalVisible(true);
                        }}
                    >
                        <PlusOutlined /> Create
                    </Button>,
                ]}
                // @ts-ignore
                request={async (params, sort, filter) => {
                    const sortField = Object.keys(sort)?.[0];
                    const sortOrder = sort?.[sortField] ?? undefined;
                    // @ts-ignore
                    const { data, code } = await postAdminList({
                        ...params,
                        sortField,
                        sortOrder,
                        ...filter,
                    } as API.PageRequest);

                    return {
                        success: code === 0,
                        data: data?.data || [],
                        total: Number(data?.total) || 0,
                    };
                }}
                columns={columns}
            />

            <UpdateModal
                visible={updateModalVisible}
                columns={columns}
                oldData={currentRow}
                onSubmit={() => {
                    setUpdateModalVisible(false);
                    setCurrentRow(undefined);
                    actionRef.current?.reload();
                }}
                onCancel={() => {
                    setUpdateModalVisible(false);
                }}
            />
        </PageContainer>
    );
};
export default UserAdminPage;