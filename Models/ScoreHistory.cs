using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class ScoreHistory
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal? AvgScore { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Usercount { get; set; }
    }
}
