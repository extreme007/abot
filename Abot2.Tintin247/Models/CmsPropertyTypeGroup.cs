using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsPropertyTypeGroup
    {
        public CmsPropertyTypeGroup()
        {
            CmsPropertyTypes = new HashSet<CmsPropertyType>();
        }

        public int Id { get; set; }
        public int ContenttypeNodeId { get; set; }
        public string Text { get; set; }
        public int Sortorder { get; set; }
        public Guid UniqueId { get; set; }

        public virtual CmsContentType ContenttypeNode { get; set; }
        public virtual ICollection<CmsPropertyType> CmsPropertyTypes { get; set; }
    }
}
