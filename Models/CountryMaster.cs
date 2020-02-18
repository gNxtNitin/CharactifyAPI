using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class CountryMaster
    {
        public CountryMaster()
        {
            StateMaster = new HashSet<StateMaster>();
        }

        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string IsoCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<StateMaster> StateMaster { get; set; }
    }
}
