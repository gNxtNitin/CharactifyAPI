﻿using System;
using System.Collections.Generic;

namespace Charactify.API.Models
{
    public partial class FeedImagePath
    {
        public int Id { get; set; }
        public int? FeedId { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Filter { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
