using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class FriendShip
    {
        public string ConnectedDate { get; set; }
        public List<FriendDetail> friendDetaillst { get; set; }
        public List<GetFeedRections> getFeedRectionslst { get; set; }
        public List<userDetail> currentUser { get; set; }
        public List<userDetail> friend { get; set; }
        public scoreRecive scoreRecives { get; set; }
        public scoreGiven scoreGiven { get; set; }
    }

    public class FriendDetail
    {
        public int userId { get; set; }

        public string userName { get; set; }

        public string useProfilePic { get; set; }
        public decimal AvgScore { get; set; }
        public string ScoreDate { get; set; }
        public List<Traits> traitslst { get; set; }
       
    }


    public class scoreGiven
    {
        public decimal AvgScore { get; set; }
        public DateTime ScoreDate { get; set; }
        public List<Traits> traitslst { get; set; }
    }

    public class scoreRecive
    {
        public decimal AvgScore { get; set; }
        public DateTime ScoreDate { get; set; }
        public List<Traits> traitslst { get; set; }
    }


    public class userDetail
    {
        public int userId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string useProfilePic { get; set; }


    }

    public class FriendShipRequest
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int currentUserId { get; set; }

        public int friendUserId { get; set; }
    }

}
