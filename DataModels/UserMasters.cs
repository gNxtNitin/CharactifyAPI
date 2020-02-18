using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class UserMasters
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public string FbuserName { get; set; }
        public string GmailUserName { get; set; }
        public string LoginKey { get; set; }
        public string Password { get; set; }
        public CountryMaster Country { get; set; }
        public StateMaster State { get; set; }
        public string UserName { get; set; }
        public string VerificationCode { get; set; }
        public string UniqueId { get; set; }
        public string AppUserName { get; set; }
    }
}
