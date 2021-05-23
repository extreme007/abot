﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUser2UserGroup
    {
        public int UserId { get; set; }
        public int UserGroupId { get; set; }

        public virtual UmbracoUser User { get; set; }
        public virtual UmbracoUserGroup UserGroup { get; set; }
    }
}
