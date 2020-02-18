using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class ApprovalsMaster
    {
        public int ApprovalsId { get; set; }
        public int UserId { get; set; }
        public int? TraitsId { get; set; }
        public string ApprovalStatus { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
