using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUserGroup2NodePermission
    {
        public int UserGroupId { get; set; }
        public int NodeId { get; set; }
        public string Permission { get; set; }

        public virtual UmbracoNode Node { get; set; }
        public virtual UmbracoUserGroup UserGroup { get; set; }
    }
}
