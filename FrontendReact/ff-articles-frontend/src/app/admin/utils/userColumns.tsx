import type { ProColumns } from "@ant-design/pro-components";
import { Typography, Space } from "antd";

export const getUserColumns = (): ProColumns<API.UserDto>[] => [
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