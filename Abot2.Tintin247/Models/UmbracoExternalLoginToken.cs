using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoExternalLoginToken
    {
        public int Id { get; set; }
        public int ExternalLoginId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual UmbracoExternalLogin ExternalLogin { get; set; }
    }
}
