using System;
using System.Collections.Generic;

namespace Charactify.API.DataModels
{
    public partial class Configurations
    {
        public int ConfigurationsId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
