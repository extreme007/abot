using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoCacheInstruction
    {
        public int Id { get; set; }
        public DateTime UtcStamp { get; set; }
        public string JsonInstruction { get; set; }
        public string Originated { get; set; }
        public int InstructionCount { get; set; }
    }
}
