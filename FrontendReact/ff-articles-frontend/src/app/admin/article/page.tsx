"use client";
import CreateModal from "./components/CreateModal";
import UpdateModal from "./components/UpdateModal";

import { PlusOutlined } from "@ant-design/icons";
import type { ActionType, ProColumns } from "@ant-design/pro-components";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button, message, Space, Typography } from "antd";
import React, { useRef, useState } from "react";
import './index.css';
import TagList from "@/components/TagList";
import MdEditor from "@/components/MdEditor";
import { apiArticleDeleteById, apiArticleGetByPage } from "@/api/contents/api/article";


const ArticleAdminPage: React.FC = () => {
    // Whether to show the create modal
    const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
    // Whether to show the update modal
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
    const actionRef = useRef<ActionType>();
    const [currentRow, setCurrentRow] = useState<API.ArticleDto>();

    /**
     * Delete a node
     *
     * @param row
     */
    const handleDelete = async (row: API.ArticleDto) => {
        const hide = message.loading("Deleting");
        if (!row) return true;
        try {
            await apiArticleDeleteById({
                id: row.articleId as any,
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
    const columns: ProColumns<API.ArticleDto>[] = [
        {
            title: "ID",
            dataIndex: "articleId",
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
            title: "Content",
            dataIndex: "content",
            valueType: "text",
            hideInTable: true,
            hideInSearch: true,
            renderFormItem: (item,
                {
                    //@ts-ignore
                    fieldProps
                }, form) => {
                return <MdEditor {...fieldProps} />;
            },
        },
        {
            title: "Topic",
            dataIndex: "topicTitle",
            valueType: "text",
            fieldProps: {
                mode: "tags",
            },
            render: (_, record) => {
                const tagList = record.topicTitle ? [record.topicTitle] : [];
                return <TagList tagList={tagList} />;
            },
            hideInForm: true,
            sorter: true,
        },
        {
            title: "Tags",
            dataIndex: "tags",
            valueType: "text",
            fieldProps: {
                mode: "tags",
            },
            render: (_, record) => {
                const tagList = record.tags ? record.tags : [];
                return <TagList tagList={tagList} />;
            },
            hideInForm: true,
        },
        {
            title: "Sort Num",
            dataIndex: "sortNumber",
            valueType: "text",

            sorter: true,
        },
        {
            title: "Hidden",
            dataIndex: "isHidden",
            valueType: "text",
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: "Created Time",
            sorter: true,
            dataIndex: "createTime",
            valueType: "date",
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: "Updated Time",
            sorter: true,
            dataIndex: "updateTime",
            valueType: "date",
            hideInSearch: true,
            hideInForm: true,
        },
        {
            title: "Actions",
            dataIndex: "option",
            valueType: "option",
            width: 130,
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
            <ProTable<API.ArticleDto>
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
                    const response = await apiArticleGetByPage({
                        PageNumber:params.current,
                        PageSize:params.pageSize,
                        sortField,
                        sortOrder,
                        ...filter,
                    } as API.apiArticleGetByPageParams);
                    //@ts-ignore
                    const { data, code } = response;
                    return {
                        success: code === 0,
                        data: data?.data || [],
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
export default ArticleAdminPage;
