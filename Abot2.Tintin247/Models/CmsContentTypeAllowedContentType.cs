using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsContentTypeAllowedContentType
    {
        public int Id { get; set; }
        public int AllowedId { get; set; }
        public int SortOrder { get; set; }

        public virtual CmsContentType Allowed { get; set; }
        public virtual CmsContentType IdNavigation { get; set; }
    }
}
