using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class Share
    {
        public int ShareId { get; set; }
        public int FeedId { get; set; }
        public int UserId { get; set; }
        public string description { get; set; }
    }
}
