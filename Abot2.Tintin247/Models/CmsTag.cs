using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsTag
    {
        public CmsTag()
        {
            CmsTagRelationships = new HashSet<CmsTagRelationship>();
        }

        public int Id { get; set; }
        public string Group { get; set; }
        public int? LanguageId { get; set; }
        public string Tag { get; set; }

        public virtual UmbracoLanguage Language { get; set; }
        public virtual ICollection<CmsTagRelationship> CmsTagRelationships { get; set; }
    }
}
