using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class DashboardData
    {
        public List<FeedStoryPost> lsttest1 { get; set; }
        public List<PostFeedReaction> lsttest2 { get; set; }
    }


    public class FeedStoryPost
    {
        public int FromUserID { get; set; }
        public string UserName { get; set; }
        public string FeedType { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }

        public string FilePath { get; set; }
        public   List <PostFeedReaction> lsttest { get; set; }
    }

    public class PostFeedReaction
    {
        public int ReactionID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int FeedID { get; set; }
        public string ReactionType { get; set; }
        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

    }
    public class PostCountFeedReaction
    {
        public int NoLike { get; set; }
        public int NoComments { get; set; }
        public int NoShares { get; set; }
        

    }
}
