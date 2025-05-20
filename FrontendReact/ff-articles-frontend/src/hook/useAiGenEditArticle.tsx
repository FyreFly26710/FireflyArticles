import { useState } from 'react';
import { useAiGenContext } from '@/states/AiGenContext';
import { useArticleContentGeneration } from './useAiGenArticle';
import { message } from 'antd';

export const useAiGenEdit = () => {
    const {
        responseData,
        setResponseData,
        setParsedArticles,
        articleListRequest,
        generationState
    } = useAiGenContext();
    const { generateArticleContent, generateAllArticles } = useArticleContentGeneration();

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
    const prettyFormatJson = (jsonStr: string): string => {
        try {
            const obj = JSON.parse(jsonStr);
            return JSON.stringify(obj, null, 2);
        } catch (e) {
            // If it's not valid JSON, return the original string
            return jsonStr;
        }
    };

    const parseArticleData = (data: string) => {
        try {
            // Attempt to parse the JSON string
            let parsedData: any = JSON.parse(data);

            // Convert UpperCamelCase keys to camelCase
            parsedData = transformKeys(parsedData);

            // Now treat it as our expected type
            const formattedData = parsedData as API.ArticlesAIResponse;

            // Validate that the parsed data has the expected structure
            if (!formattedData.articles || !Array.isArray(formattedData.articles)) {
                throw new Error('Invalid data format: articles array is missing');
            }

            setParsedArticles(formattedData);
            return formattedData;
        } catch (error) {
            console.error('Error parsing article data:', error);
            setParsedArticles(null);
            return null;
        }
    };

    const handleDataChange = (data: string) => {
        // If the data is a single line JSON without line breaks, format it prettily
        if (data.trim() && !hasLineBreaks(data)) {
            const formatted = prettyFormatJson(data);
            if (formatted !== data) {
                setResponseData(formatted);
                return;
            }
        }

        setResponseData(data);
    };

    const generateContent = async (article: API.AIGenArticleDto) => {
        if (!articleListRequest) {
            message.error('Missing article request information');
            return;
        }

        // Check if article is already being generated or has been generated
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
    };

    const generateAllContent = async () => {
        try {
            await generateAllArticles();
        } catch (error) {
            console.error('Error generating all article content:', error);
            message.error(error instanceof Error ? error.message : 'Failed to generate all article content');
        }
    };

    return {
        parseArticleData,
        handleDataChange,
        generateContent,
        generateAllContent
    };
};
