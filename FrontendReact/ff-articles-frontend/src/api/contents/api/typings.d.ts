declare namespace API {
  type ArticleAddRequest = {
    title?: string;
    content?: string;
    abstraction?: string;
    topicId?: number;
    tagIds?: number[];
    sortNumber?: number;
    isHidden?: number;
  };

  type ArticleEditRequest = {
    articleId?: number;
    title?: string;
    content?: string;
    abstraction?: string;
    topicId?: number;
    tagIds?: number[];
    sortNumber?: number;
    isHidden?: number;
  };

  type ArticleResponse = {
    articleId?: number;
    createTime?: string;
    updateTime?: string;
    title?: string;
    content?: string;
    abstraction?: string;
    userId?: number;
    user?: UserDto;
    topicId?: number;
    topicTitle?: string;
    tags?: string[];
    sortNumber?: number;
    isHidden?: number;
  };

  type ArticleResponseApiResponse = {
    code?: number;
    data?: ArticleResponse;
    message?: string;
  };

  type ArticleResponsePageResponse = {
    pageIndex?: number;
    pageSize?: number;
    recordCount?: number;
    data?: ArticleResponse[];
  };

  type ArticleResponsePageResponseApiResponse = {
    code?: number;
    data?: ArticleResponsePageResponse;
    message?: string;
  };

  type BooleanApiResponse = {
    code?: number;
    data?: boolean;
    message?: string;
  };

  type DeleteByIdRequest = {
    id?: number;
  };

  type getArticleGetIdParams = {
    id: number;
  };

  type getTagGetIdParams = {
    id: number;
  };

  type getTopicGetIdParams = {
    id: number;
  };

  type Int32ApiResponse = {
    code?: number;
    data?: number;
    message?: string;
  };

  type PageRequest = {
    pageNumber?: number;
    pageSize?: number;
    sortField?: string;
    sortOrder?: string;
  };

  type TagAddRequest = {
    tagName?: string;
  };

  type TagEditRequest = {
    tagId?: number;
    tagName?: string;
  };

  type TagResponse = {
    tagId?: number;
    tagName?: string;
  };

  type TagResponseApiResponse = {
    code?: number;
    data?: TagResponse;
    message?: string;
  };

  type TagResponseListApiResponse = {
    code?: number;
    data?: TagResponse[];
    message?: string;
  };

  type TopicAddRequest = {
    title?: string;
    content?: string;
    abstraction?: string;
    sortNumber?: number;
    isHidden?: number;
  };

  type TopicEditRequest = {
    topicId?: number;
    title?: string;
    content?: string;
    abstraction?: string;
    sortNumber?: number;
    isHidden?: number;
  };

  type TopicResponse = {
    topicId?: number;
    createTime?: string;
    updateTime?: string;
    title?: string;
    content?: string;
    abstraction?: string;
    userId?: number;
    user?: UserDto;
    sortNumber?: number;
    isHidden?: number;
  };

  type TopicResponseApiResponse = {
    code?: number;
    data?: TopicResponse;
    message?: string;
  };

  type TopicResponsePageResponse = {
    pageIndex?: number;
    pageSize?: number;
    recordCount?: number;
    data?: TopicResponse[];
  };

  type TopicResponsePageResponseApiResponse = {
    code?: number;
    data?: TopicResponsePageResponse;
    message?: string;
  };

  type UserDto = {
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
