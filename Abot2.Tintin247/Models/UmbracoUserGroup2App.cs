using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUserGroup2App
    {
        public int UserGroupId { get; set; }
        public string App { get; set; }

        public virtual UmbracoUserGroup UserGroup { get; set; }
    }
}
