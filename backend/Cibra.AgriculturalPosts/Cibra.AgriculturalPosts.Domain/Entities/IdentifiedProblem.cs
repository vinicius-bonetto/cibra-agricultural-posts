using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cibra.AgriculturalPosts.Domain.Entities
{
    public class IdentifiedProblem
    {
        public ProblemType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
    }

    public enum ProblemType
    {
        None = 0,
        Pest = 1,
        Disease = 2,
        Weather = 3,
        Soil = 4,
        Nutrition = 5,
        Water = 6
    }
}
