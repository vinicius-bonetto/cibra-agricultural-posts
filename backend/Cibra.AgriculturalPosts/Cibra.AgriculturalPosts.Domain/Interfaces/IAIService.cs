using Cibra.AgriculturalPosts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cibra.AgriculturalPosts.Domain.Interfaces
{
    public interface IAIService
    {
        Task<PostAnalysis> AnalyzePostAsync(string content, CancellationToken cancellationToken = default);
        Task<string> ProcessUserMentionAsync(string query, string postContent, List<AIInteraction> history, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync();
    }
}
