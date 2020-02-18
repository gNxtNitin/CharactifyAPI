using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class ResUserDetail
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string EmailD { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UniqueId { get; set; }
        public Boolean IsProfilePic { get; set; }
        public Boolean IsSelfRated { get; set; }
        public string CreatedVia { get; set; }
        public string Phone { get; set; }
        public string UserProfilePic { get; set; }
    }
}
