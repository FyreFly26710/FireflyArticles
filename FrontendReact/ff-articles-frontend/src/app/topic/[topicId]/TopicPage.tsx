"use server";
/* DO NOT REMOVE OR EDIT THIS
ArticlePage&TopicPage: core function of the app
They are identitical, articlepage get article from passing articleID, Topicpage get article from passing topicId
Left: ArticleSider (layout):
- Title: Topic Titile
- Child: Artice Title, can expand if has sub articles
- can collapse to give more space for ArticleCard
- can expand to show article titles

Mid&Right: ArticleCard: 
- ArticleHeaderCard: show title, tags, abstract (render using @MdViewer)
- ArticleContentCard: display content using @MdViewer

Extra: ArticleButtons:
- float button sets on the right
- provide buttons for new page, edit, open edit modal, scroll to top

*/
import { apiTopicGetById } from '@/api/contents/api/topic';
import TopicPageClient from './TopicPageClient';
// Server component to fetch data
const TopicPage = async ({ params }: { params: { topicId: string } }) => {
    let topic: API.TopicDto | null = null;
    let error = null;
    
    try {
        const response = await apiTopicGetById({
            id: parseInt(params.topicId),
            IncludeUser: true,
            IncludeArticles: true,
            IncludeSubArticles: true,
            IncludeContent: true
        });
        
        if (response.data) {
            topic = response.data;
        }
    } catch (err) {
        error = 'Failed to fetch topic data';
        console.error(error, err);
    }

    // Pass the server-fetched data to the client component
    return <TopicPageClient topic={topic} error={error} />;
};

export default TopicPage;