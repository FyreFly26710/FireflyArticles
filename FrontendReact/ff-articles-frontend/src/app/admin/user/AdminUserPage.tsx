"use client";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button } from "antd";
import React, { useRef } from "react";
import { getUserColumns } from "@/app/(admingroup)/admin/user/userColumns";
import { apiUserGetByPage } from "@/api/identity/api/user";

const AdminUserPage: React.FC = () => {
    const actionRef = useRef<any>();

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