using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoContent
    {
        public UmbracoContent()
        {
            CmsContentNus = new HashSet<CmsContentNu>();
            CmsTagRelationships = new HashSet<CmsTagRelationship>();
            UmbracoContentSchedules = new HashSet<UmbracoContentSchedule>();
            UmbracoContentVersions = new HashSet<UmbracoContentVersion>();
        }

        public int NodeId { get; set; }
        public int ContentTypeId { get; set; }

        public virtual CmsContentType ContentType { get; set; }
        public virtual UmbracoNode Node { get; set; }
        public virtual CmsMember CmsMember { get; set; }
        public virtual UmbracoDocument UmbracoDocument { get; set; }
        public virtual ICollection<CmsContentNu> CmsContentNus { get; set; }
        public virtual ICollection<CmsTagRelationship> CmsTagRelationships { get; set; }
        public virtual ICollection<UmbracoContentSchedule> UmbracoContentSchedules { get; set; }
        public virtual ICollection<UmbracoContentVersion> UmbracoContentVersions { get; set; }
    }
}
