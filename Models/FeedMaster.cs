using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class FeedMaster
    {
        public int FeedId { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public string FeedType { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public int? Crid { get; set; }
        public bool? IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
