// import { useCallback } from 'react';
// import { useDispatch, useSelector } from 'react-redux';
// import { RootState } from '@/states/reduxStore';
// import { startEditing, updateEditingArticle, cancelEditing } from '@/states/reduxStore';
// import { apiArticleEditByRequest } from '@/api/contents/api/article';
// import { message } from 'antd';

// export const useArticleEdit = () => {
//     const dispatch = useDispatch();
//     const { isEditing, currentArticle } = useSelector((state: RootState) => state.editArticle);

//     const handleStartEditing = useCallback((article: API.ArticleDto) => {
//         dispatch(startEditing(article));
//     }, [dispatch]);

//     const handleUpdateArticle = useCallback((updates: Partial<API.ArticleDto>) => {
//         dispatch(updateEditingArticle(updates));
//     }, [dispatch]);

//     const handleCancelEditing = useCallback(() => {
//         dispatch(cancelEditing());
//     }, [dispatch]);

//     const handleSubmitEdit = useCallback(async () => {
//         if (!currentArticle) return;

//         try {
//             const editRequest: API.ArticleEditRequest = {
//                 articleId: currentArticle.articleId,
//                 title: currentArticle.title,
//                 content: currentArticle.content,
//                 abstract: currentArticle.abstract,
//                 tags: currentArticle.tags,
//             };

//             const response = await apiArticleEditByRequest(editRequest);

//             if (response.data) {
//                 message.success('Article updated successfully');
//                 dispatch(cancelEditing());
//                 return true;
//             } else {
//                 message.error(response.message || 'Failed to update article');
//                 return false;
//             }
//         } catch (error) {
//             message.error('An error occurred while updating the article');
//             return false;
//         }
//     }, [currentArticle, dispatch]);

//     return {
//         isEditing,
//         currentArticle,
//         startEditing: handleStartEditing,
//         updateArticle: handleUpdateArticle,
//         cancelEditing: handleCancelEditing,
//         submitEdit: handleSubmitEdit,
//     };
// }; 