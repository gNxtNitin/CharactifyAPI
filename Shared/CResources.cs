using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API
{
    public static class CResources
    {
        public static IHostingEnvironment env { get; set; }
        public static IConfiguration configuration { get; set; }
        public static IConfiguration MySettings { get; set; }
        public static string GetConnectionString()
        {
            return configuration.GetConnectionString("ConnStr");
        }

        public static Boolean Apilog()
        {
            return Convert.ToBoolean(configuration.GetSection("MySettings").GetSection("ApilogFlag").Value);
        }

        public static int CardWhite()
        {
            return Convert.ToInt32(configuration.GetSection("MySettings").GetSection("CardWhite").Value);
        }

        public static int CardGray()
        {
            return Convert.ToInt32(configuration.GetSection("MySettings").GetSection("CardGray").Value);
        }

        public static int CardGolden()
        {
            return Convert.ToInt32(configuration.GetSection("MySettings").GetSection("CardGolden").Value);
        }
        
    }
}
