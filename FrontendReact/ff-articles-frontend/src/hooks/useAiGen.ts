import { useDispatch, useSelector } from 'react-redux';
import { message } from 'antd';
import { RootState } from '@/stores';
import {
    setLoading,
    setResponseData,
    setParsedArticles,
    setArticleListRequest,
    setArticleGenerationState,
    clearResults
} from '@/stores/aiGenSlice';
import {
    apiAiArticlesGenerateList,
    apiAiArticlesGenerateContent
} from '@/api/ai/api/aiarticles';

// Helper functions
const toCamelCase = (str: string): string => {
    return str.charAt(0).toLowerCase() + str.slice(1);
};

const transformKeys = (obj: any): any => {
    if (obj === null || typeof obj !== 'object') {
        return obj;
    }

    if (Array.isArray(obj)) {
        return obj.map(item => transformKeys(item));
    }

    return Object.keys(obj).reduce((acc, key) => {
        const camelKey = toCamelCase(key);
        acc[camelKey] = transformKeys(obj[key]);
        return acc;
    }, {} as Record<string, any>);
};

const hasLineBreaks = (str: string): boolean => {
    return str.includes('\n') || str.includes('\r');
};

const prettyFormatJson = (str: string): string => {
    try {
        const obj = JSON.parse(str);
        return JSON.stringify(obj, null, 2);
    } catch {
        return str;
    }
};

export const useAiGen = () => {
    const dispatch = useDispatch();
    const {
        loading,
        responseData,
        parsedArticles,
        articleListRequest,
        generationState
    } = useSelector((state: RootState) => state.aiGen);

    // Article List Generation
    const generateArticles = async (request: API.ArticleListRequest) => {
        try {
            dispatch(setLoading(true));
            dispatch(setArticleListRequest(request));

            const response = await apiAiArticlesGenerateList(request);
            if (response.code !== 200) {
                throw new Error(response.message);
            }

            const jsonData = response.data;
            if (typeof jsonData !== 'string') {
                throw new Error('Invalid response: Expected string data');
            }

            dispatch(setResponseData(jsonData));
            return jsonData;
        } catch (error) {
            console.error('Error in generateArticles:', error);
            message.error(error instanceof Error ? error.message : 'An unknown error occurred');
            throw error;
        } finally {
            dispatch(setLoading(false));
        }
    };

    // Single Article Content Generation
    const generateArticleContent = async (article: API.AIGenArticleDto): Promise<number | undefined> => {
        if (!articleListRequest || !parsedArticles) {
            message.error('Missing required data to generate article content');
            return undefined;
        }

        try {
            dispatch(setArticleGenerationState({
                sortNumber: article.sortNumber,
                state: { loading: true }
            }));

            const contentRequest: API.ContentRequest = {
                sortNumber: article.sortNumber,
                category: articleListRequest.category,
                title: article.title,
                abstract: article.abstract,
                tags: article.tags,
                topic: articleListRequest.topic,
                topicAbstract: articleListRequest.topicAbstract,
                topicId: parsedArticles.topicId,
                provider: articleListRequest.provider
            };

            const response = await apiAiArticlesGenerateContent(contentRequest);
            const articleId = response.data;

            dispatch(setArticleGenerationState({
                sortNumber: article.sortNumber,
                state: { loading: false, articleId }
            }));

            message.success(`Successfully generated content for "${article.title}"`);
            return articleId;
        } catch (error) {
            console.error(`Error generating content for article ${article.sortNumber}:`, error);

            dispatch(setArticleGenerationState({
                sortNumber: article.sortNumber,
                state: {
                    loading: false,
                    error: error instanceof Error ? error.message : 'Failed to generate content'
                }
            }));

            message.error(`Failed to generate content for "${article.title}"`);
            return undefined;
        }
    };

    // Generate All Pending Articles
    const generateAllArticles = async () => {
        if (!articleListRequest || !parsedArticles) {
            message.error('Missing required data to generate article content');
            return;
        }

        const pendingArticles = parsedArticles.articles.filter(article => {
            const state = generationState[article.sortNumber];
            return !state || (state.loading === false && typeof state.articleId !== 'number');
        });

        if (pendingArticles.length === 0) {
            message.info('All articles have already been generated or are in progress');
            return;
        }

        message.info(`Generating ${pendingArticles.length} pending articles...`);

        const generationPromises = pendingArticles.map(article => generateArticleContent(article));
        const results = await Promise.all(generationPromises);

        const successCount = results.filter(result => result !== undefined).length;
        if (successCount > 0) {
            message.success(`Successfully generated ${successCount} out of ${pendingArticles.length} articles!`);
        } else if (pendingArticles.length > 0) {
            message.error('Failed to generate any articles');
        }
    };

    // Response Data Handling
    const parseArticleData = (data: string) => {
        try {
            let parsedData: any = JSON.parse(data);
            parsedData = transformKeys(parsedData);

            const formattedData = parsedData as API.ArticlesAIResponse;

            if (!formattedData.articles || !Array.isArray(formattedData.articles)) {
                throw new Error('Invalid data format: articles array is missing');
            }

            dispatch(setParsedArticles(formattedData));
            return formattedData;
        } catch (error) {
            console.error('Error parsing article data:', error);
            dispatch(setParsedArticles(null));
            return null;
        }
    };

    const handleDataChange = (data: string) => {
        if (data.trim() && !hasLineBreaks(data)) {
            const formatted = prettyFormatJson(data);
            if (formatted !== data) {
                dispatch(setResponseData(formatted));
                return;
            }
        }
        dispatch(setResponseData(data));
    };

    const clearGenerationResults = () => {
        dispatch(clearResults());
    };

    return {
        // State
        loading,
        responseData,
        parsedArticles,
        articleListRequest,
        generationState,

        // Actions
        generateArticles,
        generateArticleContent,
        generateAllArticles,
        parseArticleData,
        handleDataChange,
        clearGenerationResults
    };
}; 