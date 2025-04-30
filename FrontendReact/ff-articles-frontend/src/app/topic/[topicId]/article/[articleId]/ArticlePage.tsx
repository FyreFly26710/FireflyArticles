"use server";

/* DO NOT REMOVE OR EDIT THIS
ArticlePage&TopicPage: core function of the app
They are identitical, articlepage get article from passing articleID, Topicpage get article from passing topicId
Left: ArticleSider:
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

ArticleSider and ArticleCard are restrict by .max-width-content css class in @globals.css

Key point: these pages need to use ssr. performance is the most important thing.
I need pages to render as fast as possible when users click on the sider 


Follow the instructions, i need you to write the logics how you want to implemant it as comments before coding, 
inlcuding components, hooks/ssr fetchers, when to call api, how to complete functions for interactivity while ensuring ssr performance, and folder structure. 
write all in comments at the bottom 
*/


const ArticlePage = ({ params }: { params: { topicId: number, articleId: number } }) => {
   
    return (
        <div>
            <h1>Article Page</h1>
        </div>
    );
}

export default ArticlePage;



/*



*/

