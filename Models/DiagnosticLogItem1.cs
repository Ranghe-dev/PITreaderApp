using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Models
{
    public class DiagnosticLogItem
    {
        public int Id { get; set; }

        public int ChangeOfState { get; set; }

        public DateTime Timestamp { get; set; }

        public int Index { get; set; }

        public List<DiagnosticLogParam> Params { get; set; } = new();
    }
}
