using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class CategoryScoreHistory
    {
        public int Id { get; set; }
        public string CatId { get; set; }
        public int? ScoreHistoryId { get; set; }
        public decimal? SelfAvgScore { get; set; }
        public decimal? FamilyAvgScore { get; set; }
        public decimal? CoWorkerAvgScore { get; set; }
        public decimal? AcquaintancesAvgScore { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public decimal? FriendAvgScore { get; set; }
    }
}
