using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http.Filters;

namespace Charactify.API.Shared
{
    public class GetFeedList
    {
        public int UserId { get; set; }

        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public bool Story { get; set; }

        public int FeedId { get; set; }
    }


    public class VerifyEmailOrPhone
    {
         public int Userid { get; set; }
        public string Type { get; set; }
    }
    public class GetVideo
    {
        string Url { get; set; }
    }
    public class myActionFilter : Attribute, IActionFilter
    {
         
    }
}
