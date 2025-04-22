export interface EditableArticle extends API.AIGenArticleDto {
  isEditing: boolean;
}

export interface ArticleGenerationStatus {
  [articleId: number]: {
    isGenerating: boolean;
    generatedArticleId?: number;
  };
}
