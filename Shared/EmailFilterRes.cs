using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class EmailFilterRes
    {
        public EmailStatus emailStatus
        { get; set; }
    }
    public class EmailStatus
    {
        public int Userid { get; set; }
        public string EmailID { get; set; }
        
        public bool Status { get; set; }
        public List<SearchList> SearchList { get; set; }
    }


    public class EmailStatus1
    {
        public int Userid { get; set; }
        public List<string> EmailID { get; set; }
        public bool Status { get; set; }
        public List<SearchList> SearchList { get; set; }
    }
   

    public class SearchList
    {
        public int Userid { get; set; }
        public List<Email> emails { get; set; }       
        public string Username { get; set; }
    }

    public class Email
    {
        public string emailid { get; set; }
        public string name { get; set; }
        public string Phone { get; set; }
    }
    //public class RootObject
    //{
    //    public List<SearchList> SearchList { get; set; }
    //}

}
