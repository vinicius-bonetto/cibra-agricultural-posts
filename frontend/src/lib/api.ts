import axios from 'axios';
import {
  Post,
  CreatePostRequest,
  UpdatePostRequest,
  AIInteraction,
  PagedResponse,
} from '@/types/post';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

const apiClient = axios.create({
  baseURL: `${API_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    // Add auth token here if needed
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

export const postsApi = {
  // Get all posts with pagination
  getAll: async (page: number = 1, pageSize: number = 10): Promise<PagedResponse<Post>> => {
    const response = await apiClient.get('/posts', {
      params: { page, pageSize },
    });
    return response.data;
  },

  // Get post by ID
  getById: async (id: string): Promise<Post> => {
    const response = await apiClient.get(`/posts/${id}`);
    return response.data;
  },

  // Get posts by user ID
  getByUserId: async (userId: string): Promise<Post[]> => {
    const response = await apiClient.get(`/posts/user/${userId}`);
    return response.data;
  },

  // Create new post
  create: async (data: CreatePostRequest): Promise<Post> => {
    const response = await apiClient.post('/posts', data);
    return response.data;
  },

  // Update post
  update: async (id: string, data: UpdatePostRequest): Promise<Post> => {
    const response = await apiClient.put(`/posts/${id}`, data);
    return response.data;
  },

  // Mention AI assistant
  mentionAI: async (postId: string, query: string): Promise<AIInteraction> => {
    const response = await apiClient.post(`/posts/${postId}/mention`, { query });
    return response.data;
  },
};

export default apiClient;