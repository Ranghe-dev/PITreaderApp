using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Models
{
    public class Authentication
    {
        public bool Authenticated { get; set; }
        public int AuthenticationStatus { get; set; }
        public int FailureReason { get; set; }
        public int Permission { get; set; }
        public string SecurityId { get; set; }
        public string TransponderUid { get; set; }
    }
}
