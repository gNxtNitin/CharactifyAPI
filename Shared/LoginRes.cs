using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class LoginRes
    {
        public Int32 USerID { get; set; }
        public string UserName { get; set; }
        public Boolean IsProfilepic { get; set; }
        public Boolean Isselfrated { get; set; }
    }
}
