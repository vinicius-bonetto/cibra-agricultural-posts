using Cibra.AgriculturalPosts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cibra.AgriculturalPosts.Domain.Interfaces
{
    public interface IAIInteractionRepository
    {
        Task SaveInteractionAsync(Guid postId, AIInteraction interaction, CancellationToken cancellationToken = default);
        Task<List<AIInteraction>> GetInteractionsByPostIdAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<List<AIInteraction>> GetRecentInteractionsAsync(int limit = 100, CancellationToken cancellationToken = default);
    }
}
