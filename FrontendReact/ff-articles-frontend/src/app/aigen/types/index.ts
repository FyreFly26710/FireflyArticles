export interface EditableArticle extends API.AIGenArticleDto {
  isEditing: boolean;
}

export interface ArticleGenerationStatus {
  [articleId: number]: {
    isGenerating: boolean;
    generatedArticleId?: number;
  };
}

export interface ContentRequestPayload {
  id: number;
  title: string;
  abstract: string;
  tags: string[];
  topic: string;
  topicId: number;
} 