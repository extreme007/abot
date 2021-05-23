using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsContentType2ContentType
    {
        public int ParentContentTypeId { get; set; }
        public int ChildContentTypeId { get; set; }

        public virtual UmbracoNode ChildContentType { get; set; }
        public virtual UmbracoNode ParentContentType { get; set; }
    }
}
