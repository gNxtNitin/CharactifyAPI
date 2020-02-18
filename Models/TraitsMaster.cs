using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class TraitsMaster
    {
        public int TraitsId { get; set; }
        public string TraitName { get; set; }
        public decimal? MinimumScore { get; set; }
        public decimal? MaximumScore { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserId { get; set; }
    }
}
