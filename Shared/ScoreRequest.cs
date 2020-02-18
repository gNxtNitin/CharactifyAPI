using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class ScoreRequest
    {
        
        public int FromUserID { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Status { get; set; }      
        public int ToUserID { get; set; }
        public int UserID { get; set; }
        public int CRID { get; set; }
        public string RequestType { get; set; }
        public int RequestId { get; set; }
        public int currentUser { get; set; }
        public bool mute { get; set; }

        public int connectionid { get; set; }
        public List<TraitReq> ScoreTrait { get; set; }   
        

    }

    public class TraitReq
    {
        public int TraitID { get; set; }
        public string Score { get; set; }
    }
}
