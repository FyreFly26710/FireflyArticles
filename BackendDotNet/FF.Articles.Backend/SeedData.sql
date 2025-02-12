
USE FireflyApp
GO
TRUNCATE TABLE users

INSERT INTO Users(UserAccount,UserPassword,UserName,UserProfile,UserRole,CreateTime,UpdateTime)
VALUES 
('firefly', '0732d07230a94c9318ecdfc223dfb310', 'Firefly Admin', 'Website host', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
('test1', '0732d07230a94c9318ecdfc223dfb310', 'test Admin', 'developer', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
('test2', '0732d07230a94c9318ecdfc223dfb310', 'test Admin2', 'admin', 'admin','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638'),
('test3', '0732d07230a94c9318ecdfc223dfb310', 'test user', 'default user', 'user','2025-02-12 20:03:15.1447983','2025-02-12 20:03:15.1448638');


-- Insert into Contents.Tag
INSERT INTO Contents.Tag (TagName)
VALUES 
    ('Technology'),
    ('Science'),
    ('Health'),
    ('Education');

-- Insert into Contents.Topic
INSERT INTO Contents.Topic (Title, Content, Abstraction, UserId, SortNumber, IsHidden, CreateTime, UpdateTime, IsDelete)
VALUES
    ('Tech Innovations', 'Content about technology innovations...', 'Brief on tech innovations', (FLOOR(RAND() * 3) + 1), 1, 0, GETDATE(), GETDATE(), 0),
    ('Medical Breakthroughs', 'Content about medical research...', 'Overview of medical breakthroughs', (FLOOR(RAND() * 3) + 1), 2, 0, GETDATE(), GETDATE(), 0),
    ('Future of Education', 'Content about the future of education...', 'Insights into educational reforms', (FLOOR(RAND() * 3) + 1), 3, 0, GETDATE(), GETDATE(), 0);

-- Insert into Contents.Article
INSERT INTO Contents.Article (Title, Content, Abstraction, UserId, TopicId, SortNumber, IsHidden, CreateTime, UpdateTime, IsDelete)
VALUES
    ('Tech Trends 2025', 'Article content discussing upcoming tech trends in 2025...', 'Short summary of tech trends', (FLOOR(RAND() * 3) + 1), 1, 1, 0, GETDATE(), GETDATE(), 0),
    ('Medical Advancements in 2025', 'Article about advancements in the medical field...', 'Summary of health breakthroughs', (FLOOR(RAND() * 3) + 1), 2, 2, 0, GETDATE(), GETDATE(), 0),
    ('AI and the Future of Education', 'Article on the influence of AI in education...', 'Brief on AI impacts on education', (FLOOR(RAND() * 3) + 1), 3, 3, 0, GETDATE(), GETDATE(), 0);

-- Insert into Contents.ArticleTag
INSERT INTO Contents.ArticleTag (ArticleId, TagId)
VALUES
    (1, 1), -- Article 1, Tag 1 (Technology)
    (1, 2), -- Article 1, Tag 2 (Science)
    (2, 3), -- Article 2, Tag 3 (Health)
    (3, 4); -- Article 3, Tag 4 (Education)
