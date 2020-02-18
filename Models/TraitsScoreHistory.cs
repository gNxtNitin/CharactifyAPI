using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class TraitsScoreHistory
    {
        public int Id { get; set; }
        public int? CatScoreHistoryId { get; set; }
        public decimal? HonestAvgScore { get; set; }
        public decimal? HonorableAvgScore { get; set; }
        public decimal? RespectfulAvgScore { get; set; }
        public decimal? FairAvgScore { get; set; }
        public decimal? ForgivingAvgScore { get; set; }
        public decimal? GenerousAvgScore { get; set; }
        public decimal? CourageousAvgScore { get; set; }
        public decimal? PoliteAvgScore { get; set; }
        public decimal? LovingAvgScore { get; set; }
        public decimal? TrustworthyAvgScore { get; set; }
        public DateTime? Modifieddate { get; set; }
        public DateTime? Createddate { get; set; }
    }
}
