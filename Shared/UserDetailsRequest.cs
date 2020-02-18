using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API
{

    public class UserDetailsRequest
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DateOfBirth { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Gender { get; set; }

        public string AppUserName { get; set; }

    }

    public class UserWorkDetails
    {
        public int UserEducationID { get; set; }
        public int UserID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
        public string Fromdate { get; set; }
        public string ToDate { get; set; }
        public bool Isstillworking { get; set; }
        public string Description { get; set; }
        public bool Ispublic { get; set; }

    }

    public class Card
    {
        public string Link { get; set; }
        public string Msg { get; set; }
       

    }

    public class UserPrivacyDetails
    {
        public int UserId
        {
            get; set;
        }

        public int Categorycode
        {
            get; set;
        }

        public Boolean FamilyStaus
        {
            get; set;
        }
        public Boolean FriendsStaus
        {
            get; set;
        }
        public Boolean CoWorkersStaus
        {
            get; set;
        }
        public Boolean AcquaintancesStaus
        {
            get; set;
        }
        public Boolean AllCategory
        {
            get; set;
        }

        public Boolean Staus
        {
            get; set;
        }
    }
}
