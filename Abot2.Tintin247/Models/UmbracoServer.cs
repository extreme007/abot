using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoServer
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string ComputerName { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime LastNotifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsMaster { get; set; }
    }
}
