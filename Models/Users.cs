using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public string FBUserName { get; set; }
        public string GMailUserName { get; set; }
        public string Key { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string UserProfilePic { get; set; }
        public string CreatedVia { get; set; }
        public string ViaId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string UserName { get; set; }
        public Boolean IsProfilepic { get; set; }
        public Boolean Isselfrated { get; set; }
        public string type { get; set; }
        public string UniqueId { get; set; }
        public Boolean Phoneverify { get; set; }
        public Boolean Emailverify { get; set; }
        public string AppUserName { get; set; }
    }
    public class UserDetails
    {
        public List<Users> users { get; set; }
    }

    public class Notifications
    {
        public string NotificationToken { get; set; }

        public string Description { get; set; }
    }
}
