"use client";
import { PageContainer, ProColumns, ProTable } from "@ant-design/pro-components";
import { Button, Space, Typography } from "antd";
import React, { useRef } from "react";
import { apiUserGetByPage } from "@/api/identity/api/user";

const AdminUserPage: React.FC = () => {
    const actionRef = useRef<any>();
    const getUserColumns = (): ProColumns<API.UserDto>[] => [
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
            hideInSearch: true,
            hideInForm: true,
            hideInTable: true,
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
            render: () => (
                <Space size="middle">
                    <Typography.Link disabled={true}>
                        Edit
                    </Typography.Link>
                    <Typography.Link type="danger" disabled={true}>
                        Delete
                    </Typography.Link>
                </Space>
            ),
        },
    ];
    const fetchUsers = async (params: any, sort: any, filter: any) => {
        const sortField = Object.keys(sort)?.[0];
        const sortOrder = sort?.[sortField] ?? undefined;
        const { data, code } = await apiUserGetByPage({
            ...params,
            sortField,
            sortOrder,
            ...filter,
        } as API.apiUserGetByPageParams);
        return {
            success: code === 200,
            data: data?.data ?? [],
            total: Number(data?.counts) ?? 0,
        };
    };

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
                        disabled={true}
                    >
                        Create
                    </Button>,
                ]}
                request={fetchUsers}
                columns={getUserColumns()}
            />
        </PageContainer>
    );
};

export default AdminUserPage; 