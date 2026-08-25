using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Models
{
    public class DiagnosticLogResponse
    {
        public int LogSize { get; set; }

        public List<DiagnosticLogItem> Items { get; set; } = new();
    }
}
