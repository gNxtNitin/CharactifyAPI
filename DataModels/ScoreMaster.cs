using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class ScoreMaster
    {
        public int ScoreId { get; set; }
        public int UserId { get; set; }
        public int? TraitsId { get; set; }
        public decimal? Score { get; set; }
        public DateTime? ScoredDate { get; set; }
        public int? Approved { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ConnectionReqID { get; set; }
        public int FeedId { get; set; }
        public double? weightedavg { get; set; }
    }
}
