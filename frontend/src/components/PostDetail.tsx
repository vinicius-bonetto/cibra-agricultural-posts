'use client';

import { useState } from 'react';
import { Post } from '@/types/post';
import { usePostStore } from '@/store/usePostStore';
import { formatDistanceToNow } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import {
  X,
  Sprout,
  MapPin,
  AlertCircle,
  Lightbulb,
  MessageSquare,
  Send,
  Bot,
  User,
} from 'lucide-react';

interface PostDetailProps {
  post: Post;
  onClose: () => void;
}

export default function PostDetail({ post, onClose }: PostDetailProps) {
  const [query, setQuery] = useState('');
  const { mentionAI, loading } = usePostStore();

  const handleAIMention = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;

    await mentionAI(post.id, query.trim());
    setQuery('');
  };

  const getSeverityColor = (severity: string) => {
    switch (severity.toLowerCase()) {
      case 'alta':
        return 'bg-red-100 text-red-800 border-red-200';
      case 'média':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'baixa':
        return 'bg-green-100 text-green-800 border-green-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="p-6 border-b border-gray-200 flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
              <Sprout className="w-6 h-6 text-primary-600" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-gray-900">Detalhes da Postagem</h2>
              <p className="text-sm text-gray-500">
                {formatDistanceToNow(new Date(post.createdAt), {
                  addSuffix: true,
                  locale: ptBR,
                })}
              </p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-100 rounded-full transition-colors"
          >
            <X className="w-6 h-6 text-gray-600" />
          </button>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6 space-y-6">
          {/* Post Content */}
          <div>
            {post.location && (
              <div className="flex items-center text-sm text-gray-600 mb-2">
                <MapPin className="w-4 h-4 mr-1" />
                {post.location}
              </div>
            )}
            <p className="text-gray-800 text-lg">{post.content}</p>
          </div>

          {/* AI Analysis */}
          {post.analysis && (
            <div className="bg-gradient-to-br from-primary-50 to-blue-50 rounded-lg p-6 space-y-4">
              <div className="flex items-center space-x-2">
                <Bot className="w-6 h-6 text-primary-600" />
                <h3 className="text-lg font-bold text-gray-900">Análise da IA</h3>
                <span className="text-xs px-2 py-1 bg-white rounded-full text-gray-600">
                  {Math.round(post.analysis.confidenceScore * 100)}% confiança
                </span>
              </div>

              {/* Culture and Stage */}
              <div className="grid grid-cols-2 gap-4">
                <div className="bg-white rounded-lg p-4">
                  <p className="text-sm text-gray-600 mb-1">Cultura</p>
                  <p className="font-semibold text-gray-900">{post.analysis.cultureType}</p>
                </div>
                <div className="bg-white rounded-lg p-4">
                  <p className="text-sm text-gray-600 mb-1">Estágio</p>
                  <p className="font-semibold text-gray-900">{post.analysis.stage}</p>
                </div>
              </div>

              {/* Problems */}
              {post.analysis.problems.length > 0 && (
                <div>
                  <div className="flex items-center space-x-2 mb-3">
                    <AlertCircle className="w-5 h-5 text-red-600" />
                    <h4 className="font-semibold text-gray-900">Problemas Identificados</h4>
                  </div>
                  <div className="space-y-2">
                    {post.analysis.problems.map((problem, idx) => (
                      <div
                        key={idx}
                        className={`p-4 rounded-lg border ${getSeverityColor(problem.severity)}`}
                      >
                        <div className="flex items-center justify-between mb-1">
                          <p className="font-semibold">{problem.type}</p>
                          <span className="text-xs font-medium px-2 py-1 bg-white rounded">
                            {problem.severity}
                          </span>
                        </div>
                        <p className="text-sm">{problem.description}</p>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Recommendations */}
              {post.analysis.recommendations.length > 0 && (
                <div>
                  <div className="flex items-center space-x-2 mb-3">
                    <Lightbulb className="w-5 h-5 text-yellow-600" />
                    <h4 className="font-semibold text-gray-900">Recomendações</h4>
                  </div>
                  <ul className="space-y-2">
                    {post.analysis.recommendations.map((rec, idx) => (
                      <li key={idx} className="flex items-start space-x-2 bg-white p-3 rounded-lg">
                        <span className="text-primary-600 font-bold">•</span>
                        <span className="text-gray-800">{rec}</span>
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          )}

          {/* AI Interactions */}
          {post.interactions.length > 0 && (
            <div>
              <div className="flex items-center space-x-2 mb-4">
                <MessageSquare className="w-5 h-5 text-blue-600" />
                <h3 className="text-lg font-bold text-gray-900">Conversa com @AssistenteIA</h3>
              </div>
              <div className="space-y-3">
                {post.interactions.map((interaction) => (
                  <div key={interaction.id} className="space-y-2">
                    {/* User Query */}
                    <div className="flex items-start space-x-2">
                      <div className="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                        <User className="w-4 h-4 text-blue-600" />
                      </div>
                      <div className="bg-blue-50 rounded-lg p-3 flex-1">
                        <p className="text-sm text-gray-800">{interaction.userQuery}</p>
                      </div>
                    </div>
                    {/* AI Response */}
                    <div className="flex items-start space-x-2">
                      <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center flex-shrink-0">
                        <Bot className="w-4 h-4 text-primary-600" />
                      </div>
                      <div className="bg-primary-50 rounded-lg p-3 flex-1">
                        <p className="text-sm text-gray-800 whitespace-pre-wrap">
                          {interaction.aiResponse}
                        </p>
                        <p className="text-xs text-gray-500 mt-2">
                          {formatDistanceToNow(new Date(interaction.createdAt), {
                            addSuffix: true,
                            locale: ptBR,
                          })}
                        </p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* AI Mention Form */}
        <div className="p-6 border-t border-gray-200 bg-gray-50">
          <form onSubmit={handleAIMention} className="flex space-x-2">
            <input
              type="text"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="@AssistenteIA, me ajude com..."
              className="flex-1 px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              disabled={loading}
            />
            <button
              type="submit"
              disabled={loading || !query.trim()}
              className="bg-primary-600 hover:bg-primary-700 disabled:bg-gray-300 disabled:cursor-not-allowed text-white font-semibold py-3 px-6 rounded-lg transition-colors flex items-center space-x-2"
            >
              {loading ? (
                <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
              ) : (
                <Send className="w-5 h-5" />
              )}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}