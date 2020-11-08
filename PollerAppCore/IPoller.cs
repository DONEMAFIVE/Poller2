using System;
using System.Collections.Generic;
using System.Text;

namespace PollerAppCore
{
    public interface IPoller<T>
    {
        IPollerSettings Settings { get; }

        IObservable<T> State { get; }

        Func<T> GetCurrentState { get; }
    }
}
