import { Typography, Space, Tag } from "antd";
import type { ColumnsType } from "antd/es/table";

export const getTopicColumns = (
    onEdit: (row: API.TopicDto) => void,
    setUpdateModalVisible: (visible: boolean) => void,
    handleDelete: (row: API.TopicDto) => Promise<boolean>
): ColumnsType<API.TopicDto> => [
        {
            title: "ID",
            dataIndex: "topicId",
            key: "topicId",
            width: 80,
        },
        {
            title: "Title",
            dataIndex: "title",
            key: "title",
            ellipsis: true,
        },
        {
            title: "Abstract",
            dataIndex: "abstract",
            key: "abstract",
            ellipsis: true,
            render: (text: string) => text && text.length > 50 ? `${text.substring(0, 50)}...` : text
        },
        {
            title: "Category",
            dataIndex: "category",
            key: "category",
            sorter: (a, b) => (a.category || '').localeCompare(b.category || ''),
        },
        {
            title: "Image",
            dataIndex: "topicImage",
            key: "topicImage",
            render: (text: string) => text ? (
                <img src={text} alt="Topic" style={{ width: 64, height: 64, objectFit: 'cover' }} />
            ) : null
        },
        {
            title: "Sort Num",
            dataIndex: "sortNumber",
            key: "sortNumber",
            sorter: (a, b) => (a.sortNumber || 0) - (b.sortNumber || 0),
        },
        {
            title: "Hidden",
            dataIndex: "isHidden",
            key: "isHidden",
            render: (value: number) =>
                value === 1 ? <Tag color="red">Hidden</Tag> : <Tag color="green">Visible</Tag>
        },
        {
            title: "Created Time",
            dataIndex: "createTime",
            key: "createTime",
            render: (text: string) => text ? new Date(text).toLocaleString() : '',
            sorter: (a, b) => {
                if (!a.createTime || !b.createTime) return 0;
                return new Date(a.createTime).getTime() - new Date(b.createTime).getTime();
            }
        },
        {
            title: "Actions",
            key: "actions",
            width: 120,
            render: (_: any, record: API.TopicDto) => (
                <Space size="small">
                    <Typography.Link onClick={() => onEdit(record)}>
                        Edit
                    </Typography.Link>
                    <Typography.Link type="danger" onClick={() => handleDelete(record)}>
                        Delete
                    </Typography.Link>
                </Space>
            ),
        },
    ]; 