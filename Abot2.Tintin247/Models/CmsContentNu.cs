using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsContentNu
    {
        public int NodeId { get; set; }
        public bool Published { get; set; }
        public string Data { get; set; }
        public long Rv { get; set; }

        public virtual UmbracoContent Node { get; set; }
    }
}
