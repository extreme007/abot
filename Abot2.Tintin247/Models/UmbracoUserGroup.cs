using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUserGroup
    {
        public UmbracoUserGroup()
        {
            UmbracoUser2UserGroups = new HashSet<UmbracoUser2UserGroup>();
            UmbracoUserGroup2Apps = new HashSet<UmbracoUserGroup2App>();
            UmbracoUserGroup2NodePermissions = new HashSet<UmbracoUserGroup2NodePermission>();
        }

        public int Id { get; set; }
        public string UserGroupAlias { get; set; }
        public string UserGroupName { get; set; }
        public string UserGroupDefaultPermissions { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Icon { get; set; }
        public int? StartContentId { get; set; }
        public int? StartMediaId { get; set; }

        public virtual UmbracoNode StartContent { get; set; }
        public virtual UmbracoNode StartMedia { get; set; }
        public virtual ICollection<UmbracoUser2UserGroup> UmbracoUser2UserGroups { get; set; }
        public virtual ICollection<UmbracoUserGroup2App> UmbracoUserGroup2Apps { get; set; }
        public virtual ICollection<UmbracoUserGroup2NodePermission> UmbracoUserGroup2NodePermissions { get; set; }
    }
}
