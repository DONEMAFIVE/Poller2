using System;
using System.Collections.Generic;
using System.Text;

namespace PollerAppCore
{
    public class PollerSettings : IPollerSettings
    {
        public TimeSpan Period { get; set; }
    }
}
