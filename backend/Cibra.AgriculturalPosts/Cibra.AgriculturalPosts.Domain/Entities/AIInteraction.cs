using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cibra.AgriculturalPosts.Domain.Entities
{
    public class AIInteraction
    {
        public Guid Id { get; set; }
        public string UserQuery { get; set; } = string.Empty;
        public string AIResponse { get; set; } = string.Empty;
        public InteractionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TokensUsed { get; set; }

        public AIInteraction()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }

    public enum InteractionType
    {
        AutoAnalysis = 0,
        UserMention = 1,
        DetailedAnalysis = 2
    }
}
