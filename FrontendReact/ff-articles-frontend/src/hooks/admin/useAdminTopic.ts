"use client";
import { useEffect, useState, useCallback, useRef } from 'react';
import { message } from 'antd';
import { apiTopicGetByPage, apiTopicDeleteById } from '@/api/contents/api/topic';

interface PaginationState {
  current: number;
  pageSize: number;
  total: number;
}

interface SortParamsType {
  [key: string]: 'ascend' | 'descend' | null | undefined;
}

export const useAdminTopic = () => {
  const [topics, setTopics] = useState<API.TopicDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState<PaginationState>({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [searchText, setSearchText] = useState('');
  const [modalVisible, setModalVisible] = useState(false);
  const [modalMode, setModalMode] = useState<'add' | 'edit'>('add');
  const [currentTopic, setCurrentTopic] = useState<API.TopicDto | null>(null);
  const searchDebounceTimer = useRef<NodeJS.Timeout | null>(null);

  const fetchTopicsData = useCallback(async (
    page = pagination.current,
    pageSize = pagination.pageSize,
    keyword = searchText,
    sortParams: SortParamsType = {}
  ) => {
    setLoading(true);
    try {
      const sortField = Object.keys(sortParams)?.[0];
      const sortOrder = sortField ? sortParams[sortField] : undefined;

      const response = await apiTopicGetByPage({
        PageNumber: page,
        PageSize: pageSize,
        keyword: keyword || undefined,
        SortField: sortField,
        SortOrder: sortOrder,
        IncludeContent: true,
      } as API.apiTopicGetByPageParams);

      if (response.code === 200 && response.data) {
        setTopics(response.data.data || []);
        setPagination({
          current: page,
          pageSize: pageSize,
          total: Number(response.data.counts) || 0,
        });
      } else {
        message.error(response.message || "Failed to load topics.");
        setTopics([]);
        setPagination(prev => ({ ...prev, current: 1, total: 0 }));
      }
    } catch (error: any) {
      console.error("Failed to fetch topics:", error);
      message.error(error.message || "An unexpected error occurred while fetching topics.");
      setTopics([]);
      setPagination(prev => ({ ...prev, current: 1, total: 0 }));
    } finally {
      setLoading(false);
    }
  }, [pagination.current, pagination.pageSize, searchText]);

  useEffect(() => {
    fetchTopicsData(1, pagination.pageSize, '');
  }, []);

  const handleTableChange = useCallback((newPagination: any, filters: any, sorter: any) => {
    const sortParamsForApi: SortParamsType = {};
    if (sorter.field && sorter.order) {
      const fieldKey = Array.isArray(sorter.field) ? sorter.field.join('.') : String(sorter.field);
      sortParamsForApi[fieldKey] = sorter.order;
    }
    fetchTopicsData(newPagination.current, newPagination.pageSize, searchText, sortParamsForApi);
  }, [fetchTopicsData, searchText]);

  const handleSearchTextChange = useCallback((newSearchText: string) => {
    setSearchText(newSearchText);
    if (searchDebounceTimer.current) {
      clearTimeout(searchDebounceTimer.current);
    }
    searchDebounceTimer.current = setTimeout(() => {
      fetchTopicsData(1, pagination.pageSize, newSearchText, {});
    }, 500);
  }, [fetchTopicsData, pagination.pageSize]);

  const triggerSearch = useCallback(() => {
    if (searchDebounceTimer.current) {
        clearTimeout(searchDebounceTimer.current);
    }
    fetchTopicsData(1, pagination.pageSize, searchText, {});
  }, [fetchTopicsData, pagination.pageSize, searchText]);

  const showAddModal = useCallback(() => {
    setCurrentTopic(null);
    setModalMode('add');
    setModalVisible(true);
  }, []);

  const showEditModal = useCallback((topic: API.TopicDto) => {
    setCurrentTopic(topic);
    setModalMode('edit');
    setModalVisible(true);
  }, []);

  const handleModalCancel = useCallback(() => {
    setModalVisible(false);
    setCurrentTopic(null);
  }, []);

  const handleModalSuccess = useCallback(() => {
    setModalVisible(false);
    setCurrentTopic(null);
    fetchTopicsData(pagination.current, pagination.pageSize, searchText);
  }, [fetchTopicsData, pagination.current, pagination.pageSize, searchText]);

  const handleDeleteTopic = useCallback(async (topicId: number) => {
    const hide = message.loading("Deleting topic...");
    try {
      await apiTopicDeleteById({ id: topicId });
      hide();
      message.success("Topic deleted successfully");
      if (topics.length === 1 && pagination.current > 1) {
        fetchTopicsData(pagination.current - 1, pagination.pageSize, searchText);
      } else {
        fetchTopicsData(pagination.current, pagination.pageSize, searchText);
      }
    } catch (error: any) {
      hide();
      message.error(error.message || "Failed to delete topic.");
    }
  }, [topics.length, pagination, searchText, fetchTopicsData]);

  return {
    topics,
    loading,
    pagination,
    searchText,
    modalVisible,
    modalMode,
    currentTopic,
    handleTableChange,
    handleSearchTextChange,
    triggerSearch,
    showAddModal,
    showEditModal,
    handleModalCancel,
    handleModalSuccess,
    handleDeleteTopic,
  };
};
