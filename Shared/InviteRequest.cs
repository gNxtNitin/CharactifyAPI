using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class InviteRequest
    {
        public int InviteeID { get; set; }
        public string InviteVia { get; set; }
        public string InviteViaID { get; set; }
        public string InvitedName { get; set; }
        public string InvitedPhone { get; set; }
        public string InvitedEmailID { get; set; }
        public int InviteReSent { get; set; }
    }

    
}
