import { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { message } from 'antd';
import { RootState } from '@/stores';
import {
    setResponseData,
    setParsedArticles
} from '@/stores/aiGenSlice';
import { useAiGenArticle } from './useAiGenArticle';

export const useAiGenArticleEdit = () => {
    const dispatch = useDispatch();
    const {
        responseData,
        articleListRequest,
        generationState
    } = useSelector((state: RootState) => state.aiGen);

    const { generateArticleContent, generateAllArticles } = useAiGenArticle();

    // Helper function to convert keys from UpperCamelCase to camelCase
    const toCamelCase = (str: string): string => {
        return str.charAt(0).toLowerCase() + str.slice(1);
    };

    // Helper function to transform object keys from UpperCamelCase to camelCase
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

    // Helper to check if a string contains any line breaks
    const hasLineBreaks = (str: string): boolean => {
        return str.includes('\n') || str.includes('\r');
    };

    // Helper to pretty format a JSON string
    const prettyFormatJson = (str: string): string => {
        try {
            const obj = JSON.parse(str);
            return JSON.stringify(obj, null, 2);
        } catch {
            return str;
        }
    };

    const parseArticleData = useCallback((data: string) => {
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
    }, [dispatch]);

    const handleDataChange = useCallback((data: string) => {
        if (data.trim() && !hasLineBreaks(data)) {
            const formatted = prettyFormatJson(data);
            if (formatted !== data) {
                dispatch(setResponseData(formatted));
                return;
            }
        }
        dispatch(setResponseData(data));
    }, [dispatch]);

    const generateContent = useCallback(async (article: API.AIGenArticleDto) => {
        if (!articleListRequest) {
            message.error('Missing article request information');
            return;
        }

        const articleState = generationState[article.sortNumber];
        if (articleState) {
            if (articleState.loading) {
                message.info('This article is already being generated');
                return;
            }
            if (typeof articleState.articleId === 'number') {
                message.info('This article has already been generated');
                return;
            }
        }

        try {
            await generateArticleContent(article);
        } catch (error) {
            console.error('Error generating article content:', error);
            message.error(error instanceof Error ? error.message : 'Failed to generate article content');
        }
    }, [articleListRequest, generationState, generateArticleContent]);

    const generateAllContent = useCallback(async () => {
        try {
            await generateAllArticles();
        } catch (error) {
            console.error('Error generating all article content:', error);
            message.error(error instanceof Error ? error.message : 'Failed to generate all article content');
        }
    }, [generateAllArticles]);

    return {
        // State
        responseData,
        
        // Actions
        parseArticleData,
        handleDataChange,
        generateContent,
        generateAllContent
    };
}; 