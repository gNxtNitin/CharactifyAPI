using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.DataModels
{
    public class SignUpMaster
    {
       

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNo { get; set; }
            public string FBUserName { get; set; }
            public string GMailUserName { get; set; }
            public string Key { get; set; }
            public string Password { get; set; }
            public string EmailId { get; set; }
            public string Status { get; set; }
            public string CreatedVia { get; set; }
            public string ViaId { get; set; }
            public int CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public int ModifiedBy { get; set; }
            public DateTime ModifiedDate { get; set; }
       
    }

}
