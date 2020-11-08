using System;
using System.Collections.Generic;
using System.Text;

namespace PollerConsoleApp.Data
{
    public interface IPollerUnit
    {
        DateTimeOffset Timestamp { get; set; }
        string Content { get; set; }
    }
}
