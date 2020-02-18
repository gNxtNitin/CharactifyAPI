using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class UserAuthentication
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string SecurityQuestion1 { get; set; }
        public string SecurityQuestion2 { get; set; }
        public string SecurityQuestion3 { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public int? PasswordAttempts { get; set; }
        public int? AnswerAttempts { get; set; }
        public DateTime? ResetPasswordDate { get; set; }
        public int? IsFirstTimeLogin { get; set; }
        public DateTime? LastLogin { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long UserAuthenticationId { get; set; }
    }
}
