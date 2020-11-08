using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace PollerAppCore
{
    public class Poller<T> : IPoller<T>
    {
        public IPollerSettings Settings { get; }
        public IObservable<T> State { get; }
        public Func<T> GetCurrentState { get; }
        public Poller(IPollerSettings settings, Func<T> getCurrentState, IScheduler scheduler)
        {
            if (settings == null)
                throw new ArgumentNullException();
            if (getCurrentState == null)
                throw new ArgumentNullException();

            Settings = settings;
            GetCurrentState = getCurrentState;
            State = Observable.Interval(Settings.Period, scheduler).Select(_ => GetCurrentState.Invoke());
        }

       
    }
}
