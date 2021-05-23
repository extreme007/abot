using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int NodeId { get; set; }
        public string EntityType { get; set; }
        public DateTime Datestamp { get; set; }
        public string LogHeader { get; set; }
        public string LogComment { get; set; }
        public string Parameters { get; set; }

        public virtual UmbracoUser User { get; set; }
    }
}
