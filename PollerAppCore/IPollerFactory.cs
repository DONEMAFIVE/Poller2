using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;

namespace PollerAppCore
{
    public interface IPollerFactory<T>
    {
        IPoller<T> Create(IPollerSettings settings, Func<T> getCurrentState, IScheduler scheduler = null);
    }
}
