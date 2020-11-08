using System;
using System.Collections.Generic;
using System.Text;

namespace PollerAppCore
{
    public interface IPollerSettings
    {
        TimeSpan Period { get; }
    }
}
