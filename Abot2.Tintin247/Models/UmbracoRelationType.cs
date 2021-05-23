using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoRelationType
    {
        public UmbracoRelationType()
        {
            UmbracoRelations = new HashSet<UmbracoRelation>();
        }

        public int Id { get; set; }
        public Guid TypeUniqueId { get; set; }
        public bool Dual { get; set; }
        public Guid? ParentObjectType { get; set; }
        public Guid? ChildObjectType { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public virtual ICollection<UmbracoRelation> UmbracoRelations { get; set; }
    }
}
