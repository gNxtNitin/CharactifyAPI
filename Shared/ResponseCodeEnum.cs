using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class ResponseCodeEnum
    {
        public static int OK = 1001;
        public static int ERROR = 1002;
        public static int DBEXCEPTION = 1003;
        public static int UNHANDELEDEXCEPTION = 1004;
        public static int USERNOTFOUND = 1005;
        public static int USERALREADYEXISTS = 1006;
        public static int INVALIDPASSWORD = 1007;
        public static int intVALIDATIONFAILED = 1008;
        public static int LINKNOTAVAILABLE = 1009;
        public static int TEMPLATENOTFOUND = 1010;
        public static int MENUNOTADDED = 1011;
        public static int NORECORDFOUND = 1012;
        public static int EMAILLINKSENT = 1013;
        public static int INVALIDVERIFICATIONCODE = 1014;
        public static int UNAUTHORIZEUSER = 4001;
        public static string PASS_PHRASE = "AH!PSB0%FGHR$";
        public static string PASSWORD_KEY = "LRT%YUR#VBNL@1&2";

    }
}
