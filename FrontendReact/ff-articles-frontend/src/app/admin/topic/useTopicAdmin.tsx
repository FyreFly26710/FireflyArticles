import { useRef, useState } from "react";
import { message } from "antd";
import { apiTopicDeleteById, apiTopicGetByPage } from "@/api/contents/api/topic";

export const useTopicAdmin = () => {
    const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
    const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
    const actionRef = useRef<any>();

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

    const fetchTopics = async (params: any, sort: any, filter: any) => {
        const sortField = Object.keys(sort)?.[0];
        const sortOrder = sort?.[sortField] ?? undefined;

        const res = await apiTopicGetByPage({
            PageNumber: params.current,
            PageSize: params.pageSize,
            SortField: sortField,
            SortOrder: sortOrder,
            IncludeContent: true,
        } as API.apiTopicGetByPageParams);
        const { data, code } = res;

        return {
            success: code === 200,
            data: data?.data || [],
            total: Number(data?.counts) || 0,
        };
    };

    return {
        createModalVisible,
        setCreateModalVisible,
        updateModalVisible,
        setUpdateModalVisible,
        handleDelete,
        fetchTopics,
    };
}; 