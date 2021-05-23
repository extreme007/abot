using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsMember2MemberGroup
    {
        public int Member { get; set; }
        public int MemberGroup { get; set; }

        public virtual UmbracoNode MemberGroupNavigation { get; set; }
        public virtual CmsMember MemberNavigation { get; set; }
    }
}
