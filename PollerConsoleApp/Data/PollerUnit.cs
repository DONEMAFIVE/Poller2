using System;
using System.Collections.Generic;
using System.Text;

namespace PollerConsoleApp.Data
{
    public class PollerUnit : IPollerUnit
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Content { get; set; }
    }
}
