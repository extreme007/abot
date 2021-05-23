using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsDictionary
    {
        public CmsDictionary()
        {
            CmsLanguageTexts = new HashSet<CmsLanguageText>();
            InverseParentNavigation = new HashSet<CmsDictionary>();
        }

        public int Pk { get; set; }
        public Guid Id { get; set; }
        public Guid? Parent { get; set; }
        public string Key { get; set; }

        public virtual CmsDictionary ParentNavigation { get; set; }
        public virtual ICollection<CmsLanguageText> CmsLanguageTexts { get; set; }
        public virtual ICollection<CmsDictionary> InverseParentNavigation { get; set; }
    }
}
