"use client";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button } from "antd";
import React from "react";
import { getUserColumns } from "@/app/admin/utils/userColumns";
import { useUserAdmin } from "@/app/admin/utils/useUserAdmin";

const AdminUserPage: React.FC = () => {
    const {
        actionRef,
        fetchUsers,
    } = useUserAdmin();

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