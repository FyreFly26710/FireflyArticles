"use client";
import type { ActionType, ProColumns } from "@ant-design/pro-components";
import { PageContainer, ProTable } from "@ant-design/pro-components";
import { Button, message, Select, Space, Typography } from "antd";
import React, { useEffect, useRef, useState } from "react";
import './index.css';
import TagList from "@/components/TagList";
import MdEditor from "@/components/MdEditor";
import { apiArticleDeleteById, apiArticleGetByPage } from "@/api/contents/api/article";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import ArticleFormModal from "@/components/ArticleFormModal";
import { apiTagGetAll } from "@/api/contents/api/tag";
import TagModal from "./components/TagModal";


const ArticleAdminPage: React.FC = () => {
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
    const actionRef = useRef<ActionType>();
    const [currentRow, setCurrentRow] = useState<API.ArticleDto>();
    const [tagList, setTagList] = useState<API.TagDto[]>([]);
    const fetchTagList = async () => {
        const response = await apiTagGetAll();
        const tags = response.data || [];
        //@ts-ignore
        setTagList(tags);
    };
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

    useEffect(() => {
        //fetchTopicList();
        fetchTagList();
    }, []);

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
            hideInForm: true,
        },
        {
            title: "Content",
            dataIndex: "content",
            valueType: "text",
            hideInTable: true,
            hideInForm: true,
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
            //pass topic id
            title: "Topic",
            dataIndex: "topicId",
            valueType: "text",
            render: (_, record) => {
                const tagList = record.topicTitle ? [record.topicTitle] : [];
                return <TagList tagList={tagList} />;
            },
            sorter: true,
        },
        {
            //pass tag list
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

    const [isTagModalVisible, setIsTagModalVisible] = useState(false);

    const handleTagsChange = async () => {
        await fetchTagList();
    };

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
                        color="default"
                        variant="outlined"
                        key="primary"
                        onClick={() => {
                            setIsTagModalVisible(true);
                        }}
                    >
                        Manage Tags
                    </Button>,
                ]}
                //@ts-ignore
                request={async (params, sort, filter) => {
                    const sortField = Object.keys(sort)?.[0];
                    const sortOrder = sort?.[sortField] ?? undefined;
                    const response = await apiArticleGetByPage({
                        PageNumber: params.current,
                        PageSize: params.pageSize,
                        DisplaySubArticles: true,
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

            <ArticleFormModal
                parentArticleList={[]}
                tagList={tagList}
                modifyContent={false}
                currentArticle={currentRow as API.ArticleDto}
                visible={updateModalVisible}
                onSubmit={() => { setUpdateModalVisible(false); actionRef.current?.reload(); }}
                onCancel={() => { setUpdateModalVisible(false); }}
            />

            <Button onClick={() => setIsTagModalVisible(true)}>Manage Tags</Button>
            <TagModal
                visible={isTagModalVisible}
                tags={tagList}
                onCancel={() => setIsTagModalVisible(false)}
                onTagsChange={handleTagsChange}
            />
        </PageContainer>
    );
};
export default ArticleAdminPage;
