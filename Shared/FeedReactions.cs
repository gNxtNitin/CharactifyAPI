using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify
{
    public class FeedReactionRequest
    {
        public int ReactionID { get; set; }
        public int UserID { get; set; }
        public int FeedID { get; set; }
        public string ReactionType { get; set; }
        public string Description { get; set; }
       
    }




}
