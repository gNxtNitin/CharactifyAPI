using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class UserPrivacyDetails1
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public bool? FamilyStaus { get; set; }
        public bool? FriendsStaus { get; set; }
        public bool? CoWorkersStaus { get; set; }
        public bool? AcquaintancesStaus { get; set; }
        public bool? AllCategory { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
