using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class InvitesMaster
    {
        public int InvitesId { get; set; }
        public int InviteeId { get; set; }
        public string InviteVia { get; set; }
        public string InviteViaId { get; set; }
        public string InvitedName { get; set; }
        public int? InvitedPhone { get; set; }
        public string InvitedEmailId { get; set; }
        public DateTime InviteSentDate { get; set; }
        public int? InviteReSent { get; set; }
        public DateTime? InviteReSentDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? Status { get; set; }
        public string FromEmailId { get; set; }
    }
}
