﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUserStartNode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StartNode { get; set; }
        public int StartNodeType { get; set; }

        public virtual UmbracoNode StartNodeNavigation { get; set; }
        public virtual UmbracoUser User { get; set; }
    }
}
