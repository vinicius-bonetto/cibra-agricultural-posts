using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cibra.AgriculturalPosts.Domain.Entities
{
    public class PostAnalysis
    {
        public string CultureType { get; set; } = string.Empty;
        public CultivationStage Stage { get; set; }
        public List<IdentifiedProblem> Problems { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public double ConfidenceScore { get; set; }
        public DateTime AnalyzedAt { get; set; }
        public string RawAIResponse { get; set; } = string.Empty;
    }

    public enum CultivationStage
    {
        Unknown = 0,
        Planting = 1,
        Growing = 2,
        Flowering = 3,
        Harvesting = 4,
        PostHarvest = 5
    }
}
