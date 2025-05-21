import { useCallback, useEffect, useState } from 'react';
import { Form, message } from 'antd';
import { apiTopicGetByPage } from '@/api/contents/api/topic';
import { getTopicsByCategory } from '@/libs/utils/articleUtils';
import { useAiGen } from './useAiGen';

export const useAiGenForm = () => {
    const [form] = Form.useForm<API.ArticleListRequest>();
    const { loading, generateArticles } = useAiGen();
    
    // State
    const [categories, setCategories] = useState<string[]>([]);
    const [loadingCategories, setLoadingCategories] = useState(false);
    const [topics, setTopics] = useState<API.TopicDto[]>([]);
    const [topicsByCategory, setTopicsByCategory] = useState<Record<string, API.TopicDto[]>>({});
    const [topicOptions, setTopicOptions] = useState<{ value: string }[]>([]);
    const [drawerVisible, setDrawerVisible] = useState(false);
    const [formData, setFormData] = useState<API.ArticleListRequest | null>(null);

    // Model options
    const modelOptions = [
        { label: 'DeepSeek-V3', provider: 'deepseek' },
        { label: 'Gemini 2.5', provider: 'gemini' }
    ];

    // Fetch topics on mount
    useEffect(() => {
        fetchTopics();
    }, []);

    const fetchTopics = async () => {
        setLoadingCategories(true);
        try {
            const response = await apiTopicGetByPage({
                OnlyCategoryTopic: true,
                PageSize: 100,
            });

            if (response.data?.data) {
                const topicsData = response.data.data;

                // Extract unique categories
                const uniqueCategories = Array.from(
                    new Set(
                        topicsData
                            .map((topic: API.TopicDto) => topic.category)
                            .filter(Boolean) as string[]
                    )
                ).sort();

                setTopics(topicsData);
                setCategories(uniqueCategories);

                // Use the utility function to organize topics by category
                const topicsByCat = getTopicsByCategory(topicsData);
                setTopicsByCategory(topicsByCat);
            }
        } catch (error) {
            console.error('Failed to fetch topics:', error);
        } finally {
            setLoadingCategories(false);
        }
    };

    const handleCategoryChange = useCallback((categoryValue: string) => {
        form.setFieldsValue({ topic: undefined });

        // Update topic options based on selected category
        if (categoryValue && topicsByCategory[categoryValue]) {
            const options = topicsByCategory[categoryValue].map(topic => ({
                value: topic.title || ''
            }));
            setTopicOptions(options);
        } else {
            setTopicOptions([]);
        }
    }, [form, topicsByCategory]);

    const handleSubmit = useCallback(async (values: API.ArticleListRequest) => {
        try {
            await generateArticles(values);
        } catch (error) {
            console.error('Failed to generate articles:', error);
        }
    }, [generateArticles]);

    const openPromptDrawer = useCallback(async () => {
        try {
            const values = await form.validateFields();
            setFormData(values);
            setDrawerVisible(true);
        } catch (error) {
            console.error('Validation failed:', error);
        }
    }, [form]);

    const handlePromptConfirm = useCallback(async () => {
        if (!formData) return;

        try {
            await generateArticles(formData);
            setDrawerVisible(false);
        } catch (error) {
            console.error('Failed to generate articles after prompt confirmation:', error);
        }
    }, [formData, generateArticles]);

    const handleModelChange = useCallback((e: any) => {
        const selectedModel = modelOptions.find(model => model.provider === e.target.value);
        if (selectedModel) {
            form.setFieldsValue({
                provider: selectedModel.provider
            });
        }
    }, [form]);

    const closePromptDrawer = useCallback(() => {
        setDrawerVisible(false);
    }, []);

    return {
        // Form
        form,
        loading,
        loadingCategories,

        // Data
        categories,
        topicOptions,
        modelOptions,
        formData,
        drawerVisible,

        // Handlers
        handleCategoryChange,
        handleSubmit,
        handleModelChange,
        handlePromptConfirm,
        openPromptDrawer,
        closePromptDrawer
    };
}; 