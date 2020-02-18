using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class FeedReactions
    {
        public int ReactionId { get; set; }
        public int FeedId { get; set; }
        public int? ReactionTypeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Description { get; set; }
        public bool? IsDelete { get; set; }
    }
}
