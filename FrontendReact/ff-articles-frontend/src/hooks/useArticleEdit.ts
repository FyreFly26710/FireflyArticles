import { useDispatch, useSelector } from 'react-redux';
import { message } from 'antd';
import { RootState } from '@/stores';
import {
    startEditing,
    updateEditingArticle,
    cancelEditing
} from '@/stores/articleEditSlice';
import { apiArticleEditByRequest } from '@/api/contents/api/article';
import { apiTagGetAll } from '@/api/contents/api/tag';
import { useState, useEffect } from 'react';

export const useArticleEdit = () => {
    const dispatch = useDispatch();
    const { isEditing, currentArticle } = useSelector((state: RootState) => state.articleEdit);
    const [tags, setTags] = useState<API.TagDto[]>([]);

    useEffect(() => {
        const fetchTags = async () => {
            try {
                const response = await apiTagGetAll();
                if (response.data) {
                    setTags(response.data);
                }
            } catch (error) {
                setTags([]);
            }
        };
        if (isEditing) {
            fetchTags();
        }
    }, [isEditing]);

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('en-GB', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
        });
    };

    const handleStartEditing = (article: API.ArticleDto) => {
        dispatch(startEditing(article));
    };

    const handleUpdateArticle = (updates: Partial<API.ArticleDto>) => {
        dispatch(updateEditingArticle(updates));
    };

    const handleCancelEditing = () => {
        dispatch(cancelEditing());
    };

    const handleSubmitEdit = async () => {
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
    };

    return {
        // State
        isEditing,
        currentArticle,
        tags,

        // Utils
        formatDate,

        // Actions
        startEditing: handleStartEditing,
        updateArticle: handleUpdateArticle,
        cancelEditing: handleCancelEditing,
        submitEdit: handleSubmitEdit,
    };
}; 