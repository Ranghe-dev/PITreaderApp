using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Models
{
    public class DiagnosticEvent
    {
        public string Timestamp { get; set; }

        public string Event { get; set; }

        public string SecurityId { get; set; }

        public int Index { get; set; }
    }
}
