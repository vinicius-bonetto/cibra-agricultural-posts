'use client';

import { useState } from 'react';
import { usePostStore } from '@/store/usePostStore';
import { Sprout, MapPin, Send } from 'lucide-react';

export default function CreatePostForm() {
  const [content, setContent] = useState('');
  const [location, setLocation] = useState('');
  const { createPost, loading } = usePostStore();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!content.trim()) return;

    const post = await createPost({
      content: content.trim(),
      location: location.trim() || undefined,
    });

    if (post) {
      setContent('');
      setLocation('');
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 mb-6">
      <div className="flex items-center space-x-3 mb-4">
        <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
          <Sprout className="w-6 h-6 text-primary-600" />
        </div>
        <h2 className="text-xl font-bold text-gray-900">
          Criar Nova Postagem
        </h2>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Content */}
        <div>
          <label
            htmlFor="content"
            className="block text-sm font-medium text-gray-700 mb-2"
          >
            Descreva sua atividade agrícola
          </label>
          <textarea
            id="content"
            rows={5}
            value={content}
            onChange={(e) => setContent(e.target.value)}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent resize-none"
            placeholder="Ex: Plantei 50 hectares de soja hoje. O solo está bem preparado, mas notei algumas áreas com compactação. O clima está favorável para o desenvolvimento inicial..."
            required
            disabled={loading}
          />
          <p className="text-xs text-gray-500 mt-1">
            💡 A IA analisará automaticamente seu texto e fornecerá insights
          </p>
        </div>

        {/* Location */}
        <div>
          <label
            htmlFor="location"
            className="block text-sm font-medium text-gray-700 mb-2"
          >
            Localização (opcional)
          </label>
          <div className="relative">
            <MapPin className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              id="location"
              type="text"
              value={location}
              onChange={(e) => setLocation(e.target.value)}
              className="w-full pl-11 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              placeholder="Ex: Fazenda Santa Cruz, RS"
              disabled={loading}
            />
          </div>
        </div>

        {/* Submit Button */}
        <button
          type="submit"
          disabled={loading || !content.trim()}
          className="w-full bg-primary-600 hover:bg-primary-700 disabled:bg-gray-300 disabled:cursor-not-allowed text-white font-semibold py-3 px-6 rounded-lg transition-colors flex items-center justify-center space-x-2"
        >
          {loading ? (
            <>
              <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
              <span>Criando...</span>
            </>
          ) : (
            <>
              <Send className="w-5 h-5" />
              <span>Publicar e Analisar com IA</span>
            </>
          )}
        </button>
      </form>
    </div>
  );
}