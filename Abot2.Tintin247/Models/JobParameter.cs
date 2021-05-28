﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class JobParameter
    {
        public long JobId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual Job Job { get; set; }
    }
}
