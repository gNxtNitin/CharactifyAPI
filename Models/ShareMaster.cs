using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class ShareMaster
    {
        public int ShareId { get; set; }
        public int? FeedId { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public UserMaster User { get; set; }
    }
}
