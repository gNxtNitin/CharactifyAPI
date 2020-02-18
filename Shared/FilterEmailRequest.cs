using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class FilterEmailRequest
    {
        public int UserID { get; set; }
        public List<string> EmailID { get; set; }

    }
}
