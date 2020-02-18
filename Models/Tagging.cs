using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class Tagging
    {
        public int Id { get; set; }
        public int? FeedId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
