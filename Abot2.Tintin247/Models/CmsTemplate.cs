using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsTemplate
    {
        public CmsTemplate()
        {
            CmsDocumentTypes = new HashSet<CmsDocumentType>();
            UmbracoDocumentVersions = new HashSet<UmbracoDocumentVersion>();
        }

        public int Pk { get; set; }
        public int NodeId { get; set; }
        public string Alias { get; set; }

        public virtual UmbracoNode Node { get; set; }
        public virtual ICollection<CmsDocumentType> CmsDocumentTypes { get; set; }
        public virtual ICollection<UmbracoDocumentVersion> UmbracoDocumentVersions { get; set; }
    }
}
