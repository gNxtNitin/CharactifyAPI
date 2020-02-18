using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class AllCategoryUsers
    {
        public int UserId { get; set; }
        public string CatId { get; set; }

        public String Orderby { get; set; }
    }
}
