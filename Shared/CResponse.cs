using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class CResponse
    {
        private String msg { get; set; }
        public String data { get; set; }
        public int code { get; set; }
    }
}
