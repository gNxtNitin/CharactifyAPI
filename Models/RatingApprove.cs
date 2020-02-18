using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class RatingApprove
    {
        public int Id { get; set; }
        public int? ToUserId { get; set; }
        public int? FromUserId { get; set; }
        public string Status { get; set; }
        public int? Crid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? Modifydate { get; set; }
        public int? ModifyBy { get; set; }
    }
}
