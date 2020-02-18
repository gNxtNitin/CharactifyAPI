using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class CountryMaster
    {
        public CountryMaster()
        {
            StateMaster = new HashSet<StateMaster>();
            UserMaster = new HashSet<UserMasters>();
        }

        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string IsoCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<StateMaster> StateMaster { get; set; }
        public ICollection<UserMasters> UserMaster { get; set; }
    }
}
