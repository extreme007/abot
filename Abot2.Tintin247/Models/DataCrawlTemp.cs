﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class DataCrawlTemp
    {
        public long Id { get; set; }
        public string Aid { get; set; }
        public string Titile { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public string ThumbImage { get; set; }
        public string Link { get; set; }
        public string FullLink { get; set; }
        public string SourceImage { get; set; }
        public string SourceName { get; set; }
        public string SourceLink { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string Tags { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime PostedDatetime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsUpdated { get; set; }
    }
}
