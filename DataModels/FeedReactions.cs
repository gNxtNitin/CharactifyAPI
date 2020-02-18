using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class FeedReactions
    {
        public int ReactionId { get; set; }
        public int FeedId { get; set; }
        public string ReactionType { get; set; }
        public string Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
