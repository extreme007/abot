using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUserLogin
    {
        public Guid SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime LoggedInUtc { get; set; }
        public DateTime LastValidatedUtc { get; set; }
        public DateTime? LoggedOutUtc { get; set; }
        public string IpAddress { get; set; }

        public virtual UmbracoUser User { get; set; }
    }
}
