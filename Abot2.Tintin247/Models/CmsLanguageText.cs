using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class CmsLanguageText
    {
        public int Pk { get; set; }
        public int LanguageId { get; set; }
        public Guid UniqueId { get; set; }
        public string Value { get; set; }

        public virtual UmbracoLanguage Language { get; set; }
        public virtual CmsDictionary Unique { get; set; }
    }
}
