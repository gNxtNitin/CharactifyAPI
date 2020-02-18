using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Models
{
    public class Notification
    {
       
        public int  Id { get; set; }
        public int FromUserID { get; set; }
        public string ToUserID { get; set; }
        public string message { get; set; }
        
    }
}
