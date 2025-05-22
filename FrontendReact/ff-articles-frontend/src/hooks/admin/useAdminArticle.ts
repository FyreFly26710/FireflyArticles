"use client";
import { useEffect, useState, useCallback, useRef } from 'react';
import { message } from 'antd';
import { apiArticleGetByPage, apiArticleDeleteById } from "@/api/contents/api/article";

interface PaginationState {
    current: number;
    pageSize: number;
    total: number;
}

interface SortParamsType {
    [key: string]: 'ascend' | 'descend' | null | undefined;
}

export const useAdminArticle = () => {
    const [articles, setArticles] = useState<API.ArticleDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [pagination, setPagination] = useState<PaginationState>({
        current: 1,
        pageSize: 10,
        total: 0,
    });
    const [searchText, setSearchText] = useState('');
    const [editModalVisible, setEditModalVisible] = useState(false);
    const [currentArticle, setCurrentArticle] = useState<API.ArticleDto | null>(null);

    const searchDebounceTimer = useRef<NodeJS.Timeout | null>(null);

    const fetchArticles = useCallback(async (
        page = pagination.current,
        pageSize = pagination.pageSize,
        keyword = searchText,
        sortParams: SortParamsType = {}
    ) => {
        setLoading(true);
        try {
            const sortField = Object.keys(sortParams)?.[0];
            const sortOrder = sortField ? sortParams[sortField] : undefined;

            const response = await apiArticleGetByPage({
                PageNumber: page,
                PageSize: pageSize,
                Keyword: keyword || undefined,
                SortField: sortField,
                SortOrder: sortOrder || undefined,
                IncludeUser: true,
            });

            if (response.data?.data) {
                setArticles(response.data.data);
                setPagination({
                    current: page,
                    pageSize,
                    total: response.data.counts || 0,
                });
            } else {
                message.error(response.message || "Failed to load articles.");
                setArticles([]);
                setPagination(prev => ({ ...prev, current: 1, total: 0 }));
            }
        } catch (error: any) {
            console.error("Failed to fetch articles:", error);
            message.error(error.message || "An unexpected error occurred while fetching articles.");
            setArticles([]);
            setPagination(prev => ({ ...prev, current: 1, total: 0 }));
        } finally {
            setLoading(false);
        }
    }, [pagination.current, pagination.pageSize, searchText]);


    useEffect(() => {
        fetchArticles(1, pagination.pageSize, '');
    }, []);

    const handleTableChange = useCallback((newPagination: any, filters: any, sorter: any) => {
        const sortParamsForApi: SortParamsType = {};
        if (sorter.field && sorter.order) {
            const fieldKey = Array.isArray(sorter.field) ? sorter.field.join('.') : String(sorter.field);
            sortParamsForApi[fieldKey] = sorter.order;
        }
        fetchArticles(newPagination.current, newPagination.pageSize, searchText, sortParamsForApi);
    }, [fetchArticles, searchText]);

    const handleSearchTextChange = useCallback((newSearchText: string) => {
        setSearchText(newSearchText);
        if (searchDebounceTimer.current) {
            clearTimeout(searchDebounceTimer.current);
        }
        searchDebounceTimer.current = setTimeout(() => {
            fetchArticles(1, pagination.pageSize, newSearchText);
        }, 500);
    }, [fetchArticles, pagination.pageSize]);

    const triggerSearch = useCallback(() => {
        if (searchDebounceTimer.current) {
            clearTimeout(searchDebounceTimer.current);
        }
        fetchArticles(1, pagination.pageSize, searchText);
    }, [fetchArticles, pagination.pageSize, searchText]);

    const handleDeleteArticle = useCallback(async (articleId: number) => {
        const hide = message.loading("Deleting article...");
        try {
            const response = await apiArticleDeleteById({ id: articleId });
            hide();
            if (response.code === 200) { // Assuming code 200 is success
                message.success("Article deleted successfully");
                if (articles.length === 1 && pagination.current > 1) {
                    fetchArticles(pagination.current - 1, pagination.pageSize, searchText);
                } else {
                    fetchArticles(pagination.current, pagination.pageSize, searchText);
                }
            } else {
                message.error(response.message || "Failed to delete article");
            }
        } catch (error: any) {
            hide();
            console.error("Delete error:", error);
            message.error(error.message || "Failed to delete article");
        }
    }, [articles.length, pagination, searchText, fetchArticles]);


    const showEditModal = useCallback((article: API.ArticleDto) => {
        setCurrentArticle(article);
        setEditModalVisible(true);
    }, []);

    const handleEditModalCancel = useCallback(() => {
        setEditModalVisible(false);
        setCurrentArticle(null);
    }, []);

    const handleEditModalSuccess = useCallback(() => {
        setEditModalVisible(false);
        setCurrentArticle(null);
        fetchArticles(pagination.current, pagination.pageSize, searchText); // Refresh current page
    }, [fetchArticles, pagination.current, pagination.pageSize, searchText]);

    return {
        articles,
        loading,
        pagination,
        searchText,
        editModalVisible,
        currentArticle,
        fetchArticles,
        handleTableChange,
        handleSearchTextChange,
        triggerSearch,
        handleDeleteArticle,
        showEditModal,
        handleEditModalCancel,
        handleEditModalSuccess,
    };
};
