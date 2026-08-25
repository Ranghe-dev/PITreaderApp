using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Models
{
    public class StatusResponse
    {
        public Status Status { get; set; }
        public Led Led { get; set; }
        public Config Config { get; set; }
        public Authentication Authentication { get; set; }
    }
}
