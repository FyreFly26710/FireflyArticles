'use client';

import { createContext, useContext, useState, ReactNode, useCallback } from 'react';
import { apiAiArticlesGenerateList, apiAiArticlesGenerateContent } from '@/api/ai/api/aiarticles';
import { message } from 'antd';

interface AiGenContextType {
  loading: boolean;
  results: API.ArticlesAIResponse | undefined;
  showForm: boolean;
  editableArticles: EditableArticle[];
  generationStatus: ArticleGenerationStatus;
  isGeneratingAll: boolean;

  // Article list actions
  generateArticles: (articleListRequest: API.ArticleListRequest) => Promise<void>;
  // updateResults: (updatedResults: API.ArticlesAIResponse) => void;
  //newGeneration: () => void;
  clearResults: () => void;
  // Article edit actions
  updateEditableArticles: (articles: EditableArticle[]) => void;
  handleEditArticle: (articleId: number) => void;
  handleSaveArticle: (articleId: number) => void;
  handleCancelEdit: (articleId: number) => void;
  handleUpdateField: (articleId: number, field: keyof API.AIGenArticleDto, value: any) => void;
  handleAddTag: (articleId: number, tag: string) => void;
  handleRemoveTag: (articleId: number, tagIndex: number) => void;

  // Article generation actions
  generateArticleContent: (article: EditableArticle) => Promise<number | undefined>;
  generateAllArticles: () => Promise<void>;
}

const AiGenContext = createContext<AiGenContextType | undefined>(undefined);

export interface EditableArticle extends API.AIGenArticleDto {
  isEditing: boolean;
}

export interface ArticleGenerationStatus {
  [articleId: number]: {
    isGenerating: boolean;
    generatedArticleId?: number;
  };
}

export function AiGenProvider({ children }: { children: ReactNode }) {
  // Main states
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<API.ArticlesAIResponse>();
  const [request, setRequest] = useState<API.ArticleListRequest>();
  const [showForm, setShowForm] = useState(true);

  // Article edit states
  const [topic, setTopic] = useState<string>("");
  const [editableArticles, setEditableArticles] = useState<EditableArticle[]>([]);
  const [isGeneratingAll, setIsGeneratingAll] = useState(false);
  const [generationStatus, setGenerationStatus] = useState<ArticleGenerationStatus>({});

  // Helper for updating results
  const updateEditableArticles = useCallback((articles: EditableArticle[]) => {
    setEditableArticles(articles);
  }, []);

  // Round 1: Generate article list
  const generateArticles = async (articleListRequest: API.ArticleListRequest) => {
    setLoading(true);
    setResults(undefined);
    setTopic(topic);
    setGenerationStatus({});
    setRequest(articleListRequest);
    try {
      const response = await apiAiArticlesGenerateList(articleListRequest);
      if (response.code !== 200) {
        console.log('Error generating articles:', response.message);
        throw new Error(response.message);
      }
      const data = response.data;
      setResults(data);

      // Initialize editable articles
      if (data && data.articles) {
        setEditableArticles(
          data.articles.map(article => ({
            ...article,
            isEditing: false
          }))
        );
      }

      setShowForm(false);
    } catch (error) {
      console.error('Error generating articles:', error);
      message.error('Failed to generate article suggestions.');
    } finally {
      setLoading(false);
    }
  };

  const updateResults = (updatedResults: API.ArticlesAIResponse) => {
    setResults(updatedResults);
  };


  const clearResults = () => {
    setResults(undefined);
    setRequest(undefined);
    setEditableArticles([]);
    setGenerationStatus({});
    setShowForm(true);
  };

  // Article editing functions
  const handleEditArticle = (sortNumber: number) => {
    setEditableArticles(prevArticles =>
      prevArticles.map(article =>
        article.sortNumber === sortNumber
          ? { ...article, isEditing: true }
          : article
      )
    );
  };

  const handleSaveArticle = (sortNumber: number) => {
    setEditableArticles(prevArticles => {
      const newArticles = prevArticles.map(article =>
        article.sortNumber === sortNumber
          ? { ...article, isEditing: false }
          : article
      );

      if (results) {
        // Update the results with the edited articles
        updateResults({
          ...results,
          articles: newArticles.map(({ isEditing, ...rest }) => rest)
        });
      }

      return newArticles;
    });
  };

  const handleCancelEdit = (sortNumber: number) => {
    setEditableArticles(prevArticles =>
      prevArticles.map(article => {
        if (article.sortNumber === sortNumber && results?.articles) {
          // Find the original article from results to restore its data
          const originalArticle = results.articles.find(a => a.sortNumber === sortNumber);
          return {
            ...originalArticle!,
            isEditing: false
          };
        }
        return article;
      })
    );
  };

  const handleUpdateField = (sortNumber: number, field: keyof API.AIGenArticleDto, value: any) => {
    setEditableArticles(prevArticles =>
      prevArticles.map(article =>
        article.sortNumber === sortNumber
          ? { ...article, [field]: value }
          : article
      )
    );
  };

  const handleAddTag = (sortNumber: number, tag: string) => {
    setEditableArticles(prevArticles =>
      prevArticles.map(article => {
        if (article.sortNumber === sortNumber) {
          const updatedTags = [...article.tags, tag];
          return { ...article, tags: updatedTags };
        }
        return article;
      })
    );
  };

  const handleRemoveTag = (sortNumber: number, tagIndex: number) => {
    setEditableArticles(prevArticles =>
      prevArticles.map(article => {
        if (article.sortNumber === sortNumber) {
          const updatedTags = [...article.tags];
          updatedTags.splice(tagIndex, 1);
          return { ...article, tags: updatedTags };
        }
        return article;
      })
    );
  };

  // Round 2: Generate article content
  const generateArticleContent = async (article: EditableArticle): Promise<number | undefined> => {
    try {
      // Update status to generating
      setGenerationStatus(prev => ({
        ...prev,
        [article.sortNumber]: { isGenerating: true }
      }));

      if (!results?.topicId) {
        throw new Error('Missing topicId in results');
      }

      if (!request) {
        throw new Error('Missing request');
      }

      const contentRequest: API.ContentRequest = {
        sortNumber: article.sortNumber,
        category: request.category,
        title: article.title,
        abstract: article.abstract,
        tags: article.tags,
        topic: request.topic,
        topicAbstract: request.topicAbstract,
        topicId: results.topicId,
        model: request.model,
        provider: request.provider
      };

      const articleId = (await apiAiArticlesGenerateContent(contentRequest)).data;

      // Update status with generated article ID
      setGenerationStatus(prev => ({
        ...prev,
        [article.sortNumber]: {
          isGenerating: false,
          generatedArticleId: articleId
        }
      }));

      return articleId;
    } catch (error) {
      console.error(`Error generating content for article ${article.sortNumber}:`, error);

      // Update status to show error
      setGenerationStatus(prev => ({
        ...prev,
        [article.sortNumber]: { isGenerating: false }
      }));

      message.error(`Failed to generate content for "${article.title}"`);
      return undefined;
    }
  };

  const generateAllArticles = async () => {
    try {
      setIsGeneratingAll(true);

      // Filter articles that haven't been generated yet AND aren't currently generating
      const articlesToGenerate = editableArticles.filter(
        article => !generationStatus[article.sortNumber]?.generatedArticleId &&
          !generationStatus[article.sortNumber]?.isGenerating
      );

      if (articlesToGenerate.length === 0) {
        message.info('All articles have already been generated or are in progress.');
        setIsGeneratingAll(false);
        return;
      }

      message.info(`Generating ${articlesToGenerate.length} articles for topic: "${topic}".`);

      // Generate all articles concurrently
      const generationPromises = articlesToGenerate.map(article => {
        // Return a promise that resolves to the article ID or null for errors
        return generateArticleContent(article);
      });

      // Wait for all promises to resolve
      const results = await Promise.all(generationPromises);

      // Count successful generations
      const successCount = results.filter(result => result !== null).length;
      message.success(`Successfully generated ${successCount} out of ${articlesToGenerate.length} articles!`);
    } catch (error) {
      console.error('Error in generate all:', error);
      message.error('Failed to generate some articles. Please try again.');
    } finally {
      setIsGeneratingAll(false);
    }
  };

  return (
    <AiGenContext.Provider value={{
      loading,
      results,
      showForm,
      editableArticles,
      generationStatus,
      isGeneratingAll,
      // Article list actions
      generateArticles,
      // updateResults,
      // newGeneration,
      clearResults,
      // Article edit actions
      updateEditableArticles,
      handleEditArticle,
      handleSaveArticle,
      handleCancelEdit,
      handleUpdateField,
      handleAddTag,
      handleRemoveTag,
      // Article generation actions
      generateArticleContent,
      generateAllArticles
    }}>
      {children}
    </AiGenContext.Provider>
  );
}

export function useAiGen() {
  const context = useContext(AiGenContext);
  if (context === undefined) {
    throw new Error('useAiGen must be used within an AiGenProvider');
  }
  return context;
} 