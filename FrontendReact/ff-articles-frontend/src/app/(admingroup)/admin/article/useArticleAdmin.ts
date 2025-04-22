import { message } from "antd";
import { useRef, useState } from "react";
import { apiArticleDeleteById, apiArticleGetByPage } from "@/api/contents/api/article";
import { apiTagGetAll } from "@/api/contents/api/tag";

export const useArticleAdmin = () => {
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const actionRef = useRef<any>();
  const [currentRow, setCurrentRow] = useState<API.ArticleDto>();
  const [tagList, setTagList] = useState<API.TagDto[]>([]);

  const fetchTagList = async () => {
    const response = await apiTagGetAll();
    const tags = response.data || [];
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

  const fetchArticles = async (params: any, sort: any, filter: any) => {
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

    const { data, code } = response;
    return {
      success: code === 200,
      data: data?.data || [],
      total: Number(data?.counts) || 0,
    };
  };

  return {
    updateModalVisible,
    setUpdateModalVisible,
    actionRef,
    currentRow,
    setCurrentRow,
    tagList,
    fetchTagList,
    handleDelete,
    fetchArticles,
  };
}; 