
USE FireflyApp
GO
TRUNCATE TABLE users

INSERT INTO Users(Id, UserAccount,UserPassword,UserName,UserProfile,UserRole,CreateTime,UpdateTime)
VALUES
(1,'firefly', '0732d07230a94c9318ecdfc223dfb310', 'Firefly Admin', 'Website host', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
(2,'test1', '0732d07230a94c9318ecdfc223dfb310', 'test Admin', 'developer', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
(3,'test2', '0732d07230a94c9318ecdfc223dfb310', 'test Admin2', 'admin', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
(4,'test3', '0732d07230a94c9318ecdfc223dfb310', 'test user', 'default user', 'user','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638');
-- Truncate all tables to remove existing data
TRUNCATE TABLE Contents.ArticleTag;
TRUNCATE TABLE Contents.Tag;
TRUNCATE TABLE Contents.Article;
TRUNCATE TABLE Contents.Topic;

-- Insert Programming Topics
INSERT INTO Contents.Topic (Id, Title, Abstract, Category, Content, TopicImage, UserId, IsDelete, IsHidden, SortNumber, CreateTime, UpdateTime)
VALUES 
(1,'Web Development', 'A topic about modern web development practices.', 'Programming', 'Content about web technologies like HTML, CSS, and JavaScript.', NULL, 1, 0, 0, 1, GETDATE(), GETDATE()),
(2,'Machine Learning', 'Introduction to machine learning concepts.', 'Programming', 'ML algorithms and techniques.', NULL, 1, 0, 0, 2, GETDATE(), GETDATE()),
(3,'Database Systems', 'Understanding relational and NoSQL databases.', 'Programming', 'Covers SQL, PostgreSQL, and MongoDB.', NULL, 1, 0, 0, 3, GETDATE(), GETDATE());

-- Insert Sample Articles
INSERT INTO Contents.Article (Id, Title, Abstract, Content, ArticleType, TopicId, ParentArticleId, UserId, IsDelete, IsHidden, SortNumber, CreateTime, UpdateTime)
VALUES
(1,'Introduction to HTML', 'Basics of HTML for web development.', 'Detailed content about HTML structure and elements.', 'Article', 1, NULL, 1, 0, 0, 1, GETDATE(), GETDATE()),
(2,'CSS Styling Guide', 'How to style web pages using CSS.', 'Deep dive into CSS properties and techniques.', 'Article', 1, NULL, 1, 0, 0, 2, GETDATE(), GETDATE()),
(3,'Supervised Learning', 'Overview of supervised machine learning.', 'Covers classification and regression models.', 'Article', 2, NULL, 1, 0, 0, 3, GETDATE(), GETDATE()),
(4,'SQL Basics', 'Introduction to SQL for relational databases.', 'Covers SQL syntax, queries, and database normalization.', 'Article', 3, NULL, 1, 0, 0, 4, GETDATE(), GETDATE());

-- Insert SubArticles
INSERT INTO Contents.Article (Id,Title, Abstract, Content, ArticleType, TopicId, ParentArticleId, UserId, IsDelete, IsHidden, SortNumber, CreateTime, UpdateTime)
VALUES
(5,'HTML Tags', 'Understanding different HTML tags.', 'Detailed list of commonly used HTML tags.', 'SubArticle', 1, 1, 1, 0, 0, 5, GETDATE(), GETDATE()),
(6,'CSS Grid and Flexbox', 'Modern layout techniques in CSS.', 'Comparison and implementation of Grid and Flexbox.', 'SubArticle', 1, 2, 1, 0, 0, 6, GETDATE(), GETDATE());

-- Insert Tags
INSERT INTO Contents.Tag (id,TagName)
VALUES 
(1,'HTML'), (2,'CSS'), (3,'JavaScript'), (4,'Machine Learning'), (5,'SQL'), (6,'Database');

-- Insert Article Tags (Mapping articles to tags)
INSERT INTO Contents.ArticleTag (Id,ArticleId, TagId)
VALUES
(1,1, 1), -- HTML Article tagged with HTML
(2,2, 2), -- CSS Article tagged with CSS
(3,3, 4), -- ML Article tagged with Machine Learning
(4,4, 5), -- SQL Article tagged with SQL
(5,5, 1), -- SubArticle (HTML Tags) tagged with HTML
(6,6, 2); -- SubArticle (CSS Grid and Flexbox) tagged with CSS

