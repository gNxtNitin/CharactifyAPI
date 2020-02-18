using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class StoryRequest
    {
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public string StoryType { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }

        public string FilePath { get; set; }
    }
}
