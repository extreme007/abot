using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsMember
    {
        public CmsMember()
        {
            CmsMember2MemberGroups = new HashSet<CmsMember2MemberGroup>();
        }

        public int NodeId { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string PasswordConfig { get; set; }
        public string SecurityStampToken { get; set; }
        public DateTime? EmailConfirmedDate { get; set; }

        public virtual UmbracoContent Node { get; set; }
        public virtual ICollection<CmsMember2MemberGroup> CmsMember2MemberGroups { get; set; }
    }
}
