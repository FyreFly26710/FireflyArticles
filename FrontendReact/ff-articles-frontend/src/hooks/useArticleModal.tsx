import { useCallback, useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/states/reduxStore';
import { showModal, hideModal } from '@/states/articleModal';
import { apiArticleEditByRequest } from '@/api/contents/api/article';
import { apiTagGetAll } from '@/api/contents/api/tag';
import { apiTopicGetById } from '@/api/contents/api/topic';
import { message } from 'antd';

export const useArticleModal = () => {
    const dispatch = useDispatch();
    const { isVisible, currentArticle } = useSelector(
        (state: RootState) => state.articleModal
    );

    const [tags, setTags] = useState<API.TagDto[]>([]);
    const [parentArticles, setParentArticles] = useState<API.ArticleDto[]>([]);
    const [loading, setLoading] = useState(false);

    // Open modal
    const openModal = useCallback((article: API.ArticleDto) => {
        dispatch(showModal(article));
    }, [dispatch]);

    // Fetch data when modal opens and article changes
    useEffect(() => {
        const fetchModalData = async () => {
            if (isVisible && currentArticle) {
                setLoading(true);
                try {
                    // Fetch tags
                    const tagsResponse = await apiTagGetAll();
                    if (tagsResponse.data) {
                        setTags(tagsResponse.data);
                    }

                    // Fetch topic with articles to get potential parent articles
                    if (currentArticle.topicId) {
                        const topicResponse = await apiTopicGetById({
                            id: currentArticle.topicId,
                            IncludeArticles: true,
                        });

                        if (topicResponse.data?.articles) {
                            // Filter out the current article from potential parents
                            // Only include articles of type 'Article' as parents
                            const filteredArticles = topicResponse.data.articles.filter(
                                a => a.articleId !== currentArticle.articleId && a.articleType === 'Article'
                            );
                            console.log('Found parent articles:', filteredArticles);
                            setParentArticles(filteredArticles);
                        }
                    }
                } catch (error) {
                    console.error('Error fetching modal data:', error);
                    message.error('Failed to load tags and parent articles');
                } finally {
                    setLoading(false);
                }
            }
        };

        fetchModalData();
    }, [isVisible, currentArticle]);

    // Close modal and reset state
    const closeModal = useCallback(() => {
        dispatch(hideModal());
        setTags([]);
        setParentArticles([]);
    }, [dispatch]);

    // Submit article edit
    const submitArticleEdit = useCallback(async (values: API.ArticleEditRequest) => {
        const hide = message.loading('Updating...');
        try {
            const response = await apiArticleEditByRequest({
                ...values,
                isHidden: values.isHidden ? 1 : 0
            });

            hide();
            if (response.data) {
                message.success('Update successful!');
                dispatch(hideModal());
                return true;
            } else {
                message.error(response.message || 'Update failed');
                return false;
            }
        } catch (error: any) {
            hide();
            message.error('Update failed: ' + error.message);
            return false;
        }
    }, [dispatch]);

    return {
        isVisible,
        currentArticle,
        tags,
        parentArticles,
        loading,
        openModal,
        closeModal,
        submitArticleEdit
    };
}; 