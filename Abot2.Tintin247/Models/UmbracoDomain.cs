using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoDomain
    {
        public int Id { get; set; }
        public int? DomainDefaultLanguage { get; set; }
        public int? DomainRootStructureId { get; set; }
        public string DomainName { get; set; }

        public virtual UmbracoNode DomainRootStructure { get; set; }
    }
}
