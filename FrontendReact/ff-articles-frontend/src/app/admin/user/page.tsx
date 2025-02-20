"use client";
import { PlusOutlined } from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { Button, message, Space, Typography } from 'antd';
import React, { useRef, useState } from 'react';
import UpdateModal from './components/UpdateModal';
import { apiUserDeleteById, apiUserGetByPage } from '@/api/identity/api/user';

const UserAdminPage: React.FC = () => {
    const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
    const actionRef = useRef<ActionType>();
    const [currentRow, setCurrentRow] = useState<API.UserDto>();


    const handleDelete = async (row: API.UserDto) => {
        const hide = message.loading('Deleting');
        if (!row) return true;
        try {
            await apiUserDeleteById({
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


    const columns: ProColumns<API.UserDto>[] = [
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
            <ProTable<API.UserDto>
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
                    const { data, code } = await apiUserGetByPage({
                        ...params,
                        sortField,
                        sortOrder,
                        ...filter,
                    } as API.apiUserGetByPageParams);
                    return {
                        success: code === 0,
                        data: data?.data || [],
                        // @ts-ignore
                        total: Number(data?.counts) || 0,
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