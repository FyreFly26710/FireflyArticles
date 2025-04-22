import type { ProColumns } from "@ant-design/pro-components";
import { Typography, Space } from "antd";
import MdEditor from "@/components/MdEditor";

export const getTopicColumns = (
    setCurrentRow: (row: API.TopicDto) => void,
    setUpdateModalVisible: (visible: boolean) => void,
    handleDelete: (row: API.TopicDto) => Promise<boolean>
): ProColumns<API.TopicDto>[] => [
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
            dataIndex: "abstract",
            valueType: "text",
        },
        {
            title: "Category",
            dataIndex: "category",
            valueType: "text",
            sorter: true,
        },
        {
            title: "Content",
            dataIndex: "content",
            valueType: "text",
            hideInSearch: true,
            hideInTable: true,
            renderFormItem: (item, {
                //@ts-ignore
                fieldProps
            }, form) => {
                return <MdEditor {...fieldProps} />;
            },
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
            sorter: true,
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