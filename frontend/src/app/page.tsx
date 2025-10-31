'use client';

import { useEffect, useState } from 'react';
import { usePostStore } from '@/store/usePostStore';
import CreatePostForm from '@/components/CreatePostForm';
import PostCard from '@/components/PostCard';
import PostDetail from '@/components/PostDetail';
import { Post } from '@/types/post';
import { Sprout, ChevronLeft, ChevronRight, Loader2 } from 'lucide-react';
import { Toaster } from 'react-hot-toast';

export default function Home() {
  const { posts, loading, pagination, fetchPosts } = usePostStore();
  const [selectedPost, setSelectedPost] = useState<Post | null>(null);

  useEffect(() => {
    fetchPosts();
  }, [fetchPosts]);

  const handlePageChange = (newPage: number) => {
    fetchPosts(newPage, pagination.pageSize);
  };

  return (
    <>
      <Toaster position="top-right" />
      
      <div className="min-h-screen bg-gradient-to-br from-green-50 to-blue-50">
        {/* Header */}
        <header className="bg-white shadow-sm border-b border-gray-200">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
            <div className="flex items-center space-x-3">
              <div className="w-12 h-12 bg-primary-600 rounded-lg flex items-center justify-center">
                <Sprout className="w-7 h-7 text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">Cibra Agro</h1>
                <p className="text-sm text-gray-600">Análise Inteligente de Postagens Agrícolas</p>
              </div>
            </div>
          </div>
        </header>

        {/* Main Content */}
        <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Left Column - Create Post */}
            <div className="lg:col-span-1">
              <CreatePostForm />
              
              {/* Stats */}
              <div className="bg-white rounded-lg shadow-md p-6">
                <h3 className="text-lg font-bold text-gray-900 mb-4">Estatísticas</h3>
                <div className="space-y-3">
                  <div className="flex justify-between items-center">
                    <span className="text-gray-600">Total de Postagens</span>
                    <span className="font-bold text-primary-600">{pagination.totalCount}</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-gray-600">Analisadas com IA</span>
                    <span className="font-bold text-blue-600">
                      {posts.filter(p => p.analysis).length}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            {/* Right Column - Posts Feed */}
            <div className="lg:col-span-2">
              <div className="mb-6">
                <h2 className="text-2xl font-bold text-gray-900 mb-2">Feed de Postagens</h2>
                <p className="text-gray-600">
                  Postagens recentes com análise automática de IA
                </p>
              </div>

              {loading && posts.length === 0 ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="w-8 h-8 text-primary-600 animate-spin" />
                </div>
              ) : posts.length === 0 ? (
                <div className="bg-white rounded-lg shadow-md p-12 text-center">
                  <Sprout className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                  <h3 className="text-xl font-semibold text-gray-700 mb-2">
                    Nenhuma postagem ainda
                  </h3>
                  <p className="text-gray-500">
                    Crie sua primeira postagem e veja a mágica da IA acontecer!
                  </p>
                </div>
              ) : (
                <>
                  <div className="space-y-4">
                    {posts.map((post) => (
                      <PostCard
                        key={post.id}
                        post={post}
                        onClick={() => setSelectedPost(post)}
                      />
                    ))}
                  </div>

                  {/* Pagination */}
                  {pagination.totalPages > 1 && (
                    <div className="mt-6 flex items-center justify-center space-x-2">
                      <button
                        onClick={() => handlePageChange(pagination.page - 1)}
                        disabled={pagination.page === 1}
                        className="p-2 rounded-lg bg-white shadow hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                        <ChevronLeft className="w-5 h-5" />
                      </button>
                      
                      <div className="flex items-center space-x-1">
                        {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map((page) => (
                          <button
                            key={page}
                            onClick={() => handlePageChange(page)}
                            className={`px-4 py-2 rounded-lg ${
                              page === pagination.page
                                ? 'bg-primary-600 text-white'
                                : 'bg-white text-gray-700 hover:bg-gray-50'
                            }`}
                          >
                            {page}
                          </button>
                        ))}
                      </div>

                      <button
                        onClick={() => handlePageChange(pagination.page + 1)}
                        disabled={pagination.page === pagination.totalPages}
                        className="p-2 rounded-lg bg-white shadow hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                        <ChevronRight className="w-5 h-5" />
                      </button>
                    </div>
                  )}
                </>
              )}
            </div>
          </div>
        </main>

        {/* Footer */}
        <footer className="bg-white border-t border-gray-200 mt-12">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
            <p className="text-center text-gray-600">
              🌱 <strong>Cibra</strong> - Fazemos pela sociedade. Fazemos pelo planeta. Fertilizamos parcerias para alimentar e transformar vidas.
            </p>
          </div>
        </footer>

        {/* Post Detail Modal */}
        {selectedPost && (
          <PostDetail
            post={selectedPost}
            onClose={() => setSelectedPost(null)}
          />
        )}
      </div>
    </>
  );
}