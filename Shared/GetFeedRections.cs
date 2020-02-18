using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class GetFeedRections
    {
        public int UserID { get; set; }
        public int feedID { get; set; }
        public string Name { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string feedType { get; set; }

        public string UserProfilePic { get; set; }
        public string description { get; set; }

        public string filePath { get; set; }

        public string createdDate { get; set; }

        public int noRections { get; set; }
        public int noComments { get; set; }
        public int noShares { get; set; }
        public string currentUserRection { get; set; }
        public List<filePath> filePathlst { get; set; }

        public List<UserRectionsType> userRectionsTypeLst { get; set; }
        public List<Usercomments> usercommentLst { get; set; }
        public List<CharactifyScore> charactifyScores { get; set; }
        public List<takenScore> takenScorelst { get; set; }
        public List<tagging> taggingslst { get; set; }
       

    }
    public class CharactifyScore
    {
        public string Score { get; set; }
        public string TraitsID { get; set; }
    }

    public class filePath
    {
        public string Path { get; set; }
        public string filter { get; set; }
        public string Description { get; set; }
        public string Thumbnailurl { get; set; }
    }
    public class UserRections
    {
        public int UserID { get; set; }
        public int ReactionID { get; set; }
        public string ReactionType { get; set; }
        public int feedID { get; set; }
        public string Name { get; set; }

        public string createdDate { get; set; }
        public string createdby { get; set; }

        public string UserProfilePic { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }


    }


    public class UserRectionsType
    {
        public string ReactionType { get; set; }
        public List<UserRections> userRectionsLst { get; set; }

    }


    public class Usercomments
    {
        public int UserID { get; set; }
        public int ReactionID { get; set; }
        public string ReactionType { get; set; }
        public int feedID { get; set; }
        public string Name { get; set; }
        public string createdDate { get; set; }
        public string createdby { get; set; }

        public string UserProfilePic { get; set; }

        public string Description { get; set; }
    }


    public class Story
    {
        public int Userid { get; set; }
        public string UserName { get; set; }
        public string UserProfilePic { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }

        public List<Items> items { get; set; }

    }

    public class Items
    {
        public int id { get; set; }
        public string Date { get; set; }
        public string FileType { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string Thumbnailurl { get; set; }
    }

    public class takenScore
    {
        public string UserProfilePic { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int userid { get; set; }
    }
}
