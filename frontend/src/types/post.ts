export interface Post {
  id: string;
  userId: string;
  content: string;
  createdAt: string;
  updatedAt?: string;
  status: string;
  analysis?: PostAnalysis;
  interactions: AIInteraction[];
  tags: string[];
  location?: string;
}

export interface PostAnalysis {
  cultureType: string;
  stage: string;
  problems: IdentifiedProblem[];
  recommendations: string[];
  confidenceScore: number;
  analyzedAt: string;
}

export interface IdentifiedProblem {
  type: string;
  description: string;
  severity: string;
}

export interface AIInteraction {
  id: string;
  userQuery: string;
  aiResponse: string;
  type: string;
  createdAt: string;
  tokensUsed: number;
}

export interface CreatePostRequest {
  content: string;
  location?: string;
}

export interface UpdatePostRequest {
  content: string;
}

export interface AIMentionRequest {
  postId: string;
  query: string;
}

export interface PagedResponse<T> {
  data: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}