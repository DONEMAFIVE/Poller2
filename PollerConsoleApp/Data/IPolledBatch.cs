using System;
using System.Collections.Generic;
using System.Text;

namespace PollerConsoleApp.Data
{
    public interface IPolledBatch
    {
        TimeSpan Duration { get; }
        List<IPollerUnit> Items { get; }
    }
}
