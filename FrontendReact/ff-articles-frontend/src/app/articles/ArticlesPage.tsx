import { Suspense } from "react";
import { apiArticleGetByPage } from "@/api/contents/api/article";
import ClientArticles from "./ClientArticles";
import { apiTopicGetByPage } from "@/api/contents/api/topic";
import { getTopicsByCategory } from "@/libs/utils/articleUtils";
import { apiTagGetAll } from "@/api/contents/api/tag";

/**
 * DO NOT EDIT THIS COMMENT
 *  <SearchBar />: input search keyword, search button, clear button
 *  <TopicSelector />: select topic, display category name as tab, click tab to display topic list
 *  <TagSelector />: simple dropdown selector, display all tags
 *  <SelectedOptions />: display current selected topics and tags as tag that can be removed, has a button to clear all
 *  <ArticleTable />: display article list, each item has title, abstract, tags, author, created date. apply pagination. no sort.
 * 
 *  Components: ant design components
 *  Styles:  Use inline antd or tailwind styles
 */
const ArticlesPage = async () => {
    try {
        // Fetch initial articles with server-side rendering
        const articlesResponse = await apiArticleGetByPage({
            PageNumber: 1,
            PageSize: 10,
            IncludeUser: true,
            DisplaySubArticles: true,
            SortByRelevance: true,
        });

        const articles = articlesResponse.data?.data || [];
        const totalCount = articlesResponse.data?.counts || 0;

        const topicsResponse = await apiTopicGetByPage({
            PageNumber: 1,
            PageSize: 100,
            OnlyCategoryTopic: true,
        });

        const topics = topicsResponse.data?.data || [];
        const topicsByCategory = getTopicsByCategory(topics);

        const tagsResponse = await apiTagGetAll();
        const tags = tagsResponse.data || [];
        
        
        return (
            <div className="max-width-content">                
                <Suspense fallback={<div>Loading filters...</div>}>
                    <ClientArticles initialArticles={articles} totalCount={totalCount} />
                </Suspense>
            </div>
        );
    } catch (error) {
        console.error("Failed to fetch articles:", error);
        return (
            <div className="error-container">
                <h2>Error loading articles</h2>
                <p>Something went wrong while fetching articles. Please try again later.</p>
            </div>
        );
    }
};

export default ArticlesPage;