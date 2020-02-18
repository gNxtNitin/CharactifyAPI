using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API
{
    public class UserEducationDetails
    {
        public int UserID { get; set; }
        public int UserSchoolID { get; set; }
        public string SchoolName { get; set; }
        public string TypeofSchool { get; set; }
        public string Fromdate { get; set; }
        public string ToDate { get; set; }
        public bool Isgraduated { get; set; }
        public string Description { get; set; }
        public bool Ispublic { get; set; }

    }
}
