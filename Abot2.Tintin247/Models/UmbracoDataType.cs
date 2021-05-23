using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoDataType
    {
        public UmbracoDataType()
        {
            CmsPropertyTypes = new HashSet<CmsPropertyType>();
        }

        public int NodeId { get; set; }
        public string PropertyEditorAlias { get; set; }
        public string DbType { get; set; }
        public string Config { get; set; }

        public virtual UmbracoNode Node { get; set; }
        public virtual ICollection<CmsPropertyType> CmsPropertyTypes { get; set; }
    }
}
