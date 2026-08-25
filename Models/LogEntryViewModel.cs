using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Models
{
    namespace PITreaderApp.Models
    {
        public class LogEntryViewModel
        {
            public string Timestamp { get; set; } = "";

            public string Event { get; set; } = "";

            public string Parameter { get; set; } = "";

            public int Index { get; set; }

            public string Icon { get; set; } = "ℹ";
        }
    }
}
