using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class StoryMaster
    {
        public int StoryId { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public string StoryType { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
