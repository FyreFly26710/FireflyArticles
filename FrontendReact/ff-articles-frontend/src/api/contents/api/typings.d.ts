declare namespace API {
  type apiArticleDeleteByIdParams = {
    id: number;
  };

  type apiArticleGetByIdParams = {
    id: number;
    IncludeUser?: boolean;
    IncludeSubArticles?: boolean;
    IncludeContent?: boolean;
    DisplaySubArticles?: boolean;
  };

  type apiArticleGetByPageParams = {
    ArticleId?: number;
    IncludeUser?: boolean;
    IncludeSubArticles?: boolean;
    IncludeContent?: boolean;
    DisplaySubArticles?: boolean;
    Keyword?: string;
    TopicIds?: number[];
    TagIds?: number[];
    PageNumber?: number;
    PageSize?: number;
    SortField?: string;
    SortOrder?: string;
    SortByRelevance?: boolean;
  };

  type apiTagDeleteByIdParams = {
    id: number;
  };

  type apiTagGetByIdParams = {
    id: number;
  };

  type apiTopicDeleteByIdParams = {
    id: number;
  };

  type apiTopicGetByIdParams = {
    id: number;
    IncludeUser?: boolean;
    IncludeArticles?: boolean;
    IncludeSubArticles?: boolean;
    IncludeContent?: boolean;
  };

  type apiTopicGetByPageParams = {
    TopicId?: number;
    IncludeUser?: boolean;
    IncludeArticles?: boolean;
    IncludeSubArticles?: boolean;
    // IncludeContent?: boolean;
    OnlyCategoryTopic?: boolean;
    PageNumber?: number;
    PageSize?: number;
    SortField?: string;
    SortOrder?: string;
  };

  type ArticleAddRequest = {
    title?: string;
    content?: string;
    abstract?: string;
    articleType?: string;
    parentArticleId?: number;
    topicId?: number;
    tags?: string[];
    sortNumber?: number;
    isHidden?: number;
  };

  type ArticleDto = {
    articleId: number;
    createTime?: string;
    updateTime?: string;
    title?: string;
    content?: string;
    abstract?: string;
    articleType?: string;
    parentArticleId?: number;
    subArticles?: ArticleDto[];
    userId?: number;
    user?: UserApiDto;
    topicId?: number;
    topicTitle?: string;
    tags?: string[];
    sortNumber?: number;
    isHidden?: number;
  };

  type ArticleDtoApiResponse = {
    code?: number;
    message?: string;
    data?: ArticleDto;
  };

  type ArticleDtoPaged = {
    pageIndex?: number;
    pageSize?: number;
    counts?: number;
    data?: ArticleDto[];
  };

  type ArticleDtoPagedApiResponse = {
    code?: number;
    message?: string;
    data?: ArticleDtoPaged;
  };

  type ArticleEditRequest = {
    articleId?: number;
    title?: string;
    content?: string;
    abstract?: string;
    articleType?: string;
    parentArticleId?: number;
    topicId?: number;
    tags?: string[];
    sortNumber?: number;
    isHidden?: number;
  };

  type BooleanApiResponse = {
    code?: number;
    message?: string;
    data?: boolean;
  };

  type Int32ApiResponse = {
    code?: number;
    message?: string;
    data?: number;
  };

  type TagAddRequest = {
    tagName?: string;
  };

  type TagDto = {
    tagId?: number;
    tagName?: string;
    tagGroup?: string;
    tagColour?: string;
  };

  type TagDtoApiResponse = {
    code?: number;
    message?: string;
    data?: TagDto;
  };

  type TagDtoListApiResponse = {
    code?: number;
    message?: string;
    data?: TagDto[];
  };

  type TagEditRequest = {
    tagId?: number;
    tagName?: string;
  };

  type TopicAddRequest = {
    title?: string;
    abstract?: string;
    // content?: string;
    topicImage?: string;
    category?: string;
    sortNumber?: number;
    isHidden?: number;
  };

  type TopicDto = {
    topicId?: number;
    createTime?: string;
    updateTime?: string;
    title?: string;
    abstract?: string;
    // content?: string;
    category?: string;
    topicImage?: string;
    userId?: number;
    user?: UserApiDto;
    sortNumber?: number;
    isHidden?: number;
    articles?: ArticleDto[];
  };

  type TopicDtoApiResponse = {
    code?: number;
    message?: string;
    data?: TopicDto;
  };

  type TopicDtoPaged = {
    pageIndex?: number;
    pageSize?: number;
    counts?: number;
    data?: TopicDto[];
  };

  type TopicDtoPagedApiResponse = {
    code?: number;
    message?: string;
    data?: TopicDtoPaged;
  };

  type TopicEditRequest = {
    topicId?: number;
    title?: string;
    abstract?: string;
    content?: string;
    topicImage?: string;
    category?: string;
    sortNumber?: number;
    isHidden?: number;
  };

  type UserApiDto = {
    userId?: number;
    createTime?: string;
    userAccount?: string;
    userEmail?: string;
    userName?: string;
    userAvatar?: string;
    userProfile?: string;
    userRole?: string;
  };
}
