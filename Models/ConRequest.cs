using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class ConRequest
    {
        public int Id { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedBy { get; set; }
    }
}
