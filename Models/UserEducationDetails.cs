using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class UserEducationDetails
    {
        public int UserSchoolId { get; set; }
        public int UserId { get; set; }
        public string SchoolName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? IsGraduated { get; set; }
        public string Description { get; set; }
        public string TypeOfSchool { get; set; }
        public int? IsPublic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
