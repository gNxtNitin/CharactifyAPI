using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class FeedRequest
    {
        public int FeedID { get; set; }
        public int FromUserID { get; set; }
        public string FeedType { get; set; }
        public string FileType { get; set; }
        public int ToUserID { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public List<FeedImagePath> feedImagePathslst { get; set; }
        public List<tagging> taggingslst { get; set; }

    }

    public class FeedImagePath
    {
        public int id { get; set; }
        public int Feedid { get; set; }
        public string FileType { get; set; }
        public string Fileformat { get; set; }
        public string filePath { get; set; }
        public string Filter { get; set; }

       public string Description { get; set; }
    }
    public class tagging
    {
       public int userid { get; set; }
        public int Touserid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int FeedId { get; set; }

    }

}
