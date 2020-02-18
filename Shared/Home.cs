using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class Home
    {
        public decimal AverageScoreAllTrait { get; set; }
        public int CountCharactifiers { get; set; }
        public int CountPost { get; set; }
        public bool showProfile { get; set; }

        public string UserProfilePic { get; set; }
        public List<ScorebyCategoury> ScorebyCategourylst { get; set; }

        public List<Traits> Traitslst { get; set; }
        public List<TopCharactifiers> TopCharactifierslst { get; set; }
        public List<scoreHistory> scoreHistorylst { get; set; }
        public List<ThreeMonths> threeMonthslst { get; set; }
        public List<SixMonths> sixMonthslst { get; set; }
        public List<OneYear> oneYearlst { get; set; }
        public List<Begnning> begnninglst { get; set; }

    }

    public class Traits
    {
        public int TraitsId { get; set; }
        public decimal TraitsScore { get; set; }
    }

    public class ScorebyCategoury
    {
        public int CatId { get; set; }
        public decimal CatScore { get; set; }

    }

    public class TopCharactifiers
    {
        public decimal Score { get; set; }
        public int userId { get; set; }
        public string username { get; set; }

        public string userProfilePic { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class scoreHistory
    {
        public int Score { get; set; }
        public int month { get; set; }


    }

    public class threemonths
    {
        public int value { get; set; }
        public String Monthname { get; set; }

    }

    public class ThreeMonths
    {
        public decimal value { get; set; }
        public String Monthname { get; set; }

    }

    public class SixMonths
    {
        public decimal value { get; set; }
        public String Monthname { get; set; }

    }
    public class OneYear
    {
        public decimal value { get; set; }
        public String Monthname { get; set; }

    }

    public class Begnning
    {
        public decimal value { get; set; }
        public String Monthname { get; set; }

    }

    public class GetScore
    {
        public int UserId { get; set; }

        public string CatId { get; set; }

        public string TraitId { get; set; }

    }
}
