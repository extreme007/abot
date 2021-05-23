using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoDocument
    {
        public int NodeId { get; set; }
        public bool Published { get; set; }
        public bool Edited { get; set; }

        public virtual UmbracoContent Node { get; set; }
    }
}
