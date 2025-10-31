'use client';

import { Post } from '@/types/post';
import { formatDistanceToNow } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { Sprout, MapPin, AlertCircle, CheckCircle2, Clock } from 'lucide-react';

interface PostCardProps {
  post: Post;
  onClick?: () => void;
}

export default function PostCard({ post, onClick }: PostCardProps) {
  const getSeverityColor = (severity: string) => {
    switch (severity.toLowerCase()) {
      case 'alta':
        return 'text-red-600 bg-red-50';
      case 'média':
        return 'text-yellow-600 bg-yellow-50';
      case 'baixa':
        return 'text-green-600 bg-green-50';
      default:
        return 'text-gray-600 bg-gray-50';
    }
  };

  const getStageIcon = (stage: string) => {
    return <Sprout className="w-4 h-4" />;
  };

  return (
    <div
      onClick={onClick}
      className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow cursor-pointer p-6 border border-gray-200"
    >
      {/* Header */}
      <div className="flex items-start justify-between mb-4">
        <div className="flex items-center space-x-2">
          <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
            <Sprout className="w-5 h-5 text-primary-600" />
          </div>
          <div>
            <p className="font-semibold text-gray-900">Agricultor</p>
            <p className="text-xs text-gray-500">
              {formatDistanceToNow(new Date(post.createdAt), {
                addSuffix: true,
                locale: ptBR,
              })}
            </p>
          </div>
        </div>
        
        {post.location && (
          <div className="flex items-center text-sm text-gray-600">
            <MapPin className="w-4 h-4 mr-1" />
            {post.location}
          </div>
        )}
      </div>

      {/* Content */}
      <p className="text-gray-800 mb-4 line-clamp-3">{post.content}</p>

      {/* AI Analysis */}
      {post.analysis ? (
        <div className="space-y-3 pt-4 border-t border-gray-100">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-2">
              {getStageIcon(post.analysis.stage)}
              <span className="text-sm font-medium text-gray-700">
                {post.analysis.cultureType}
              </span>
              <span className="text-xs px-2 py-1 bg-blue-50 text-blue-600 rounded-full">
                {post.analysis.stage}
              </span>
            </div>
            <div className="flex items-center text-xs text-gray-500">
              <CheckCircle2 className="w-4 h-4 mr-1 text-green-500" />
              {Math.round(post.analysis.confidenceScore * 100)}% confiança
            </div>
          </div>

          {/* Problems */}
          {post.analysis.problems.length > 0 && (
            <div className="space-y-1">
              {post.analysis.problems.slice(0, 2).map((problem, idx) => (
                <div
                  key={idx}
                  className={`flex items-start space-x-2 text-xs p-2 rounded ${getSeverityColor(
                    problem.severity
                  )}`}
                >
                  <AlertCircle className="w-4 h-4 flex-shrink-0 mt-0.5" />
                  <div>
                    <p className="font-medium">{problem.type}</p>
                    <p className="opacity-80">{problem.description}</p>
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Recommendations preview */}
          {post.analysis.recommendations.length > 0 && (
            <div className="text-xs text-gray-600">
              <p className="font-medium mb-1">💡 Recomendações:</p>
              <p className="line-clamp-2">{post.analysis.recommendations[0]}</p>
            </div>
          )}
        </div>
      ) : (
        <div className="pt-4 border-t border-gray-100">
          <div className="flex items-center space-x-2 text-sm text-gray-500">
            <Clock className="w-4 h-4 animate-spin" />
            <span>Análise de IA em andamento...</span>
          </div>
        </div>
      )}

      {/* Interactions count */}
      {post.interactions.length > 0 && (
        <div className="mt-3 text-xs text-gray-500">
          💬 {post.interactions.length} interação(ões) com IA
        </div>
      )}
    </div>
  );
}