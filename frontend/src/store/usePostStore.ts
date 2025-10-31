import { create } from 'zustand';
import { Post, CreatePostRequest, PagedResponse } from '@/types/post';
import { postsApi } from '@/lib/api';
import toast from 'react-hot-toast';

interface PostStore {
  posts: Post[];
  currentPost: Post | null;
  loading: boolean;
  error: string | null;
  pagination: {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };

  // Actions
  fetchPosts: (page?: number, pageSize?: number) => Promise<void>;
  fetchPostById: (id: string) => Promise<void>;
  createPost: (data: CreatePostRequest) => Promise<Post | null>;
  updatePost: (id: string, content: string) => Promise<void>;
  mentionAI: (postId: string, query: string) => Promise<void>;
  setCurrentPost: (post: Post | null) => void;
  clearError: () => void;
}

export const usePostStore = create<PostStore>((set, get) => ({
  posts: [],
  currentPost: null,
  loading: false,
  error: null,
  pagination: {
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
  },

  fetchPosts: async (page = 1, pageSize = 10) => {
    set({ loading: true, error: null });
    try {
      const response: PagedResponse<Post> = await postsApi.getAll(page, pageSize);
      set({
        posts: response.data,
        pagination: {
          page: response.page,
          pageSize: response.pageSize,
          totalCount: response.totalCount,
          totalPages: response.totalPages,
        },
        loading: false,
      });
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Failed to fetch posts',
        loading: false,
      });
      toast.error('Erro ao carregar postagens');
    }
  },

  fetchPostById: async (id: string) => {
    set({ loading: true, error: null });
    try {
      const post = await postsApi.getById(id);
      set({ currentPost: post, loading: false });
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Failed to fetch post',
        loading: false,
      });
      toast.error('Erro ao carregar postagem');
    }
  },

  createPost: async (data: CreatePostRequest) => {
    set({ loading: true, error: null });
    try {
      const post = await postsApi.create(data);
      
      // Add to posts list
      set((state) => ({
        posts: [post, ...state.posts],
        loading: false,
      }));

      toast.success('Postagem criada! IA está analisando...');
      
      // Refresh after 3 seconds to get AI analysis
      setTimeout(() => {
        get().fetchPostById(post.id);
      }, 3000);

      return post;
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Failed to create post',
        loading: false,
      });
      toast.error('Erro ao criar postagem');
      return null;
    }
  },

  updatePost: async (id: string, content: string) => {
    set({ loading: true, error: null });
    try {
      const updatedPost = await postsApi.update(id, { content });
      
      set((state) => ({
        posts: state.posts.map((p) => (p.id === id ? updatedPost : p)),
        currentPost: state.currentPost?.id === id ? updatedPost : state.currentPost,
        loading: false,
      }));

      toast.success('Postagem atualizada!');
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Failed to update post',
        loading: false,
      });
      toast.error('Erro ao atualizar postagem');
    }
  },

  mentionAI: async (postId: string, query: string) => {
    set({ loading: true, error: null });
    try {
      const interaction = await postsApi.mentionAI(postId, query);
      
      // Update current post with new interaction
      set((state) => {
        if (state.currentPost && state.currentPost.id === postId) {
          return {
            currentPost: {
              ...state.currentPost,
              interactions: [...state.currentPost.interactions, interaction],
            },
            loading: false,
          };
        }
        return { loading: false };
      });

      toast.success('Resposta da IA recebida!');
    } catch (error: any) {
      set({
        error: error.response?.data?.error || 'Failed to mention AI',
        loading: false,
      });
      toast.error('Erro ao consultar IA');
    }
  },

  setCurrentPost: (post: Post | null) => {
    set({ currentPost: post });
  },

  clearError: () => {
    set({ error: null });
  },
}));