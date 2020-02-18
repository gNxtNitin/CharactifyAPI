using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class TraitScoreSummary
    {
        public decimal AverageScoreAllTrait { get; set; }

        public List<ScorebyCategoury> ScorebyCategourylst { get; set; }
        public List<TopCharactifiers> TopCharactifierslst { get; set; }
        public List<scoreHistory> scoreHistorylst { get; set; }
        public List<ThreeMonths> threeMonthslst { get; set; }
        public List<SixMonths> sixMonthslst { get; set; }
        public List<OneYear> oneYearlst { get; set; }
        public List<Begnning> begnninglst { get; set; }
    }
}
