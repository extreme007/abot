using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoAccessRule
    {
        public Guid Id { get; set; }
        public Guid AccessId { get; set; }
        public string RuleValue { get; set; }
        public string RuleType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual UmbracoAccess Access { get; set; }
    }
}
