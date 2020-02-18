using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class StateMaster
    {
        public StateMaster()
        {
            UserMaster = new HashSet<UserMasters>();
        }

        public int StateId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public int CountryId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public CountryMaster Country { get; set; }
        public ICollection<UserMasters> UserMaster { get; set; }
    }
}
