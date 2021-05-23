using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoAccess
    {
        public UmbracoAccess()
        {
            UmbracoAccessRules = new HashSet<UmbracoAccessRule>();
        }

        public Guid Id { get; set; }
        public int NodeId { get; set; }
        public int LoginNodeId { get; set; }
        public int NoAccessNodeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual UmbracoNode LoginNode { get; set; }
        public virtual UmbracoNode NoAccessNode { get; set; }
        public virtual UmbracoNode Node { get; set; }
        public virtual ICollection<UmbracoAccessRule> UmbracoAccessRules { get; set; }
    }
}
