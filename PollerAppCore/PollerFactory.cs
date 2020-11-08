using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;

namespace PollerAppCore
{
    public class PollerFactory<T> : IPollerFactory<T>
    {
        public IPoller<T> Create(IPollerSettings settings, Func<T> getCurrentState, IScheduler scheduler = null)
        {
            if (scheduler == null)
                scheduler = TaskPoolScheduler.Default;
            return new Poller<T>(settings, getCurrentState, scheduler);
        }
    }
}
