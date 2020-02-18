using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class ConnectionMaster
    {
        public int ConnectionId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public DateTime? ConnectedDate { get; set; }
        public string ConnectionType { get; set; }
        public int? LengthOfRelationship { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Status { get; set; }
        public bool? IsMute { get; set; }
    }
}
