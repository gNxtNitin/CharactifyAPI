using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class UserWorkDetails
    {
        public int UserEducationId { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? IsStillWorking { get; set; }
        public string Description { get; set; }
        public int? IsPublic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
