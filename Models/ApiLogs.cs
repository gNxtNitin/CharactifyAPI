using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class ApiLogs
    {
        public int Id { get; set; }
        public string MethodName { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
