using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class TraitRequest
    {
        public string TraitName { get; set; }
        public decimal MinimumScore { get; set; }
        public decimal MaximumScore { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifyDate { get; set; }

    }

   // public List<TraitRequest> TraitRequests { get; set; }
}
