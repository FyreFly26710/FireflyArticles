import { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { message } from 'antd';
import { RootState } from '@/stores';
import {
    startEditing,
    updateEditingArticle,
    cancelEditing
} from '@/stores/articleEditSlice';
import { apiArticleEditByRequest } from '@/api/contents/api/article';

export const useArticleEdit = () => {
    const dispatch = useDispatch();
    const { isEditing, currentArticle } = useSelector((state: RootState) => state.articleEdit);

    const handleStartEditing = useCallback((article: API.ArticleDto) => {
        dispatch(startEditing(article));
    }, [dispatch]);

    const handleUpdateArticle = useCallback((updates: Partial<API.ArticleDto>) => {
        dispatch(updateEditingArticle(updates));
    }, [dispatch]);

    const handleCancelEditing = useCallback(() => {
        dispatch(cancelEditing());
    }, [dispatch]);

    const handleSubmitEdit = useCallback(async () => {
        if (!currentArticle) {
            message.error('No article is currently being edited');
            return false;
        }

        try {
            const editRequest: API.ArticleEditRequest = {
                articleId: currentArticle.articleId,
                title: currentArticle.title,
                content: currentArticle.content,
                abstract: currentArticle.abstract,
                tags: currentArticle.tags,
            };

            const response = await apiArticleEditByRequest(editRequest);

            if (response.data) {
                message.success('Article updated successfully');
                dispatch(cancelEditing());
                return true;
            } else {
                message.error(response.message || 'Failed to update article');
                return false;
            }
        } catch (error) {
            console.error('Error updating article:', error);
            message.error('An error occurred while updating the article');
            return false;
        }
    }, [currentArticle, dispatch]);

    return {
        // State
        isEditing,
        currentArticle,
        
        // Actions
        startEditing: handleStartEditing,
        updateArticle: handleUpdateArticle,
        cancelEditing: handleCancelEditing,
        submitEdit: handleSubmitEdit,
    };
}; 