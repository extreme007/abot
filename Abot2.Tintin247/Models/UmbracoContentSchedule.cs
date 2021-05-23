﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoContentSchedule
    {
        public Guid Id { get; set; }
        public int NodeId { get; set; }
        public int? LanguageId { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }

        public virtual UmbracoLanguage Language { get; set; }
        public virtual UmbracoContent Node { get; set; }
    }
}
