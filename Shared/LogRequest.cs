using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class LogRequest
    {
        public string MethodName { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public int LogId { get; set; }
        public string currentUserId { get; set; }
    }
}
