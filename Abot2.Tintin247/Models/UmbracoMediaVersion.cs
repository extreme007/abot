using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoMediaVersion
    {
        public int Id { get; set; }
        public string Path { get; set; }

        public virtual UmbracoContentVersion IdNavigation { get; set; }
    }
}
