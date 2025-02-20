"use client";
import CreateModal from "./components/CreateModal";
import UpdateModal from "./components/UpdateModal";

import { PlusOutlined } from "@ant-design/icons";
import type { ActionType, ProColumns } from "@ant-design/pro-components";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button, message, Space, Typography } from "antd";
import React, { useRef, useState } from "react";
import './index.css';
import { apiTopicDeleteById, apiTopicGetByPage } from "@/api/contents/api/topic";


const TopicAdminPage: React.FC = () => {
    // Whether to show the create modal
    const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
    // Whether to show the update modal
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
    const actionRef = useRef<ActionType>();
    const [currentRow, setCurrentRow] = useState<API.TopicDto>();

    /**
     * Delete a node
     *
     * @param row
     */
    const handleDelete = async (row: API.TopicDto) => {
        const hide = message.loading("Deleting");
        if (!row) return true;
        try {
            await apiTopicDeleteById({
                id: row.topicId as any,
            });
            hide();
            message.success("Successfully deleted");
            actionRef?.current?.reload();
            return true;
        } catch (error: any) {
            hide();
            message.error("Failed to delete, " + error.message);
            return false;
        }
    };

    /**
     * Table column configuration
     */
    const columns: ProColumns<API.TopicDto>[] = [
        {
            title: "ID",
            dataIndex: "topicId",
            valueType: "text",
            hideInForm: true,
        },
        {
            title: "Title",
            dataIndex: "title",
            valueType: "text",
        },
        {
            title: "Description",
            dataIndex: "abstraction",
            valueType: "text",
        },
        {
            title: "Image",
            dataIndex: "topicImage",
            valueType: "image",
            fieldProps: {
                width: 64,
            },
            hideInSearch: true,
        },
        {
            title: "Sort Num",
            dataIndex: "sortNumber",
            valueType: "text",
            sorter:true,
        },
        {
            title: "Created Time",
            sorter: true,
            dataIndex: "createTime",
            valueType: "dateTime",
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: "Updated Time",
            sorter: true,
            dataIndex: "updateTime",
            valueType: "dateTime",
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: "Actions",
            dataIndex: "option",
            valueType: "option",
            width:130,
            render: (_, record) => (
                <Space size="middle">
                    <Typography.Link
                        onClick={() => {
                            setCurrentRow(record);
                            setUpdateModalVisible(true);
                        }}
                    >
                        Modify
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
                        <PlusOutlined /> New
                    </Button>,
                ]}
                //@ts-ignore
                request={async (params, sort, filter) => {
                    const sortField = Object.keys(sort)?.[0];
                    const sortOrder = sort?.[sortField] ?? undefined;
                    //@ts-ignore
                    const { data, code } = await apiTopicGetByPage({
                        ...params,
                        sortField,
                        sortOrder,
                        ...filter,
                    } as API.apiTopicGetByPageParams);

                    return {
                        success: code === 0,
                        data: data?.data ||[],
                        //@ts-ignore
                        total: Number(data?.counts) || 0,
                    };
                }}
                columns={columns}
            />
            <CreateModal
                visible={createModalVisible}
                columns={columns}
                onSubmit={() => {
                    setCreateModalVisible(false);
                    actionRef.current?.reload();
                }}
                onCancel={() => {
                    setCreateModalVisible(false);
                }}
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
export default TopicAdminPage;
