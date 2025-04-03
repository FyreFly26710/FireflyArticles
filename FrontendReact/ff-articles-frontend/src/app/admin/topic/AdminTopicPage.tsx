"use client";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button } from "antd";
import React from "react";
import { getTopicColumns } from "@/app/admin/utils/topicColumns";
import { useTopicAdmin } from "@/app/admin/utils/useTopicAdmin";
import CreateTopicModal from "@/components/CreateTopicModal";
import UpdateTopicModal from "@/components/UpdateTopicModal";

const AdminTopicPage: React.FC = () => {
    const {
        createModalVisible,
        setCreateModalVisible,
        updateModalVisible,
        setUpdateModalVisible,
        actionRef,
        currentRow,
        setCurrentRow,
        handleDelete,
        fetchTopics,
    } = useTopicAdmin();

    return (
        <PageContainer>
            <ProTable<API.TopicDto>
                headerTitle={"Query Table"}
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
                        New
                    </Button>,
                ]}
                request={fetchTopics}
                columns={getTopicColumns(setCurrentRow, setUpdateModalVisible, handleDelete)}
            />

            <CreateTopicModal
                visible={createModalVisible}
                columns={getTopicColumns(setCurrentRow, setUpdateModalVisible, handleDelete)}
                onSubmit={() => {
                    setCreateModalVisible(false);
                    actionRef.current?.reload();
                }}
                onCancel={() => {
                    setCreateModalVisible(false);
                }}
            />

            <UpdateTopicModal
                visible={updateModalVisible}
                columns={getTopicColumns(setCurrentRow, setUpdateModalVisible, handleDelete)}
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

export default AdminTopicPage; 