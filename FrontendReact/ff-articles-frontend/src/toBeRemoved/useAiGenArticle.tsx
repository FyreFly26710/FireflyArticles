// import { apiAiArticlesGenerateList, apiAiArticlesGenerateContent } from '@/api/ai/api/aiarticles';
// import { message } from 'antd';
// import { useAiGenContext } from '@/states/AiGenContext';

// const useArticleGeneration = () => {
//     const { setLoading, setResponseData, setArticleListRequest } = useAiGenContext();

//     // Round 1: Generate article list
//     const generateArticles = async (articleListRequest: API.ArticleListRequest) => {
//         try {
//             setLoading(true);

//             // Save the request to context
//             setArticleListRequest(articleListRequest);

//             const response = await apiAiArticlesGenerateList(articleListRequest);
//             if (response.code !== 200) {
//                 console.log('Error generating articles:', response.message);
//                 throw new Error(response.message);
//             }

//             const jsonData = response.data;

//             // Handle the string response
//             if (typeof jsonData !== 'string') {
//                 const error = 'Invalid response: Expected string data';
//                 throw new Error(error);
//             }

//             setResponseData(jsonData);
//             return jsonData;
//         } catch (error) {
//             console.error('Error in generateArticles:', error);
//             if (error instanceof Error) {
//                 message.error(error.message);
//             } else {
//                 message.error('An unknown error occurred');
//             }
//             throw error;
//         } finally {
//             setLoading(false);
//         }
//     };

//     return {
//         generateArticles
//     };
// };

// const useArticleContentGeneration = () => {
//     const {
//         articleListRequest,
//         parsedArticles,
//         setArticleGenerationState,
//         generationState
//     } = useAiGenContext();

//     // Round 2: Generate article content for a single article
//     const generateArticleContent = async (
//         article: API.AIGenArticleDto
//     ): Promise<number | undefined> => {
//         if (!articleListRequest || !parsedArticles) {
//             message.error('Missing required data to generate article content');
//             return undefined;
//         }

//         try {
//             // Mark this article as loading
//             setArticleGenerationState(article.sortNumber, { loading: true });

//             const contentRequest: API.ContentRequest = {
//                 sortNumber: article.sortNumber,
//                 category: articleListRequest.category,
//                 title: article.title,
//                 abstract: article.abstract,
//                 tags: article.tags,
//                 topic: articleListRequest.topic,
//                 topicAbstract: articleListRequest.topicAbstract,
//                 topicId: parsedArticles.topicId,
//                 provider: articleListRequest.provider
//             };

//             const response = await apiAiArticlesGenerateContent(contentRequest);
//             const articleId = response.data;

//             // Update the generation state with the article ID
//             setArticleGenerationState(article.sortNumber, {
//                 loading: false,
//                 articleId
//             });

//             message.success(`Successfully generated content for "${article.title}"`);
//             return articleId;
//         } catch (error) {
//             console.error(`Error generating content for article ${article.sortNumber}:`, error);

//             // Update the generation state with the error
//             setArticleGenerationState(article.sortNumber, {
//                 loading: false,
//                 error: error instanceof Error ? error.message : 'Failed to generate content'
//             });

//             message.error(`Failed to generate content for "${article.title}"`);
//             return undefined;
//         }
//     };

//     const generateAllArticles = async () => {
//         if (!articleListRequest || !parsedArticles) {
//             message.error('Missing required data to generate article content');
//             return;
//         }

//         // Filter articles that haven't been generated yet and aren't currently loading
//         const pendingArticles = parsedArticles.articles.filter(article => {
//             const state = generationState[article.sortNumber];
//             // Skip if already generated (has articleId) or is currently loading
//             return !state || (state.loading === false && typeof state.articleId !== 'number');
//         });

//         if (pendingArticles.length === 0) {
//             message.info('All articles have already been generated or are in progress');
//             return;
//         }

//         message.info(`Generating ${pendingArticles.length} pending articles...`);

//         const generationPromises = pendingArticles.map(article => {
//             // Return a promise that resolves to the article ID or undefined for errors
//             return generateArticleContent(article);
//         });

//         // Wait for all promises to resolve
//         const results = await Promise.all(generationPromises);

//         // Count successful generations
//         const successCount = results.filter(result => result !== undefined).length;

//         if (successCount > 0) {
//             message.success(`Successfully generated ${successCount} out of ${pendingArticles.length} articles!`);
//         } else if (pendingArticles.length > 0) {
//             message.error('Failed to generate any articles');
//         }
//     };

//     return {
//         generateArticleContent,
//         generateAllArticles
//     };
// };

// export { useArticleGeneration, useArticleContentGeneration };
