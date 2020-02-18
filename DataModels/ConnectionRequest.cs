using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class ConnectionRequest
    {
        public int Crid { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Status { get; set; }
        public string ConnectionType { get; set; }
        public int? LengthOfRelationship { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
