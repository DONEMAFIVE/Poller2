using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using PollerAppCore;

namespace PollerConsoleApp
{
    public sealed class PolledConsumerWrapper : IDisposable
    {
        private readonly ConcurrentBag<Data.IPollerUnit> keyAccumulator = new ConcurrentBag<Data.IPollerUnit>();
        private readonly CancellationTokenSource _cancel; //Источник токена отмены
        
        public PolledConsumerWrapper(IObservable<Data.IPollerUnit> source, 
                                     TimeSpan pollingPeriod,  
                                     Action<List<Data.IPollerUnit>> actWithPolled,
                                     CancellationTokenSource cancelSource,
                                     IScheduler pollScheduler = null)
        {
            var transmittingSubscription = source.SubscribeOn(TaskPoolScheduler.Default)
                                                      .Subscribe(keyItem => keyAccumulator.Add(keyItem));

            var poller = (new PollerFactory<List<Data.IPollerUnit>>().Create(
                new PollerSettings() { Period = pollingPeriod },
                () =>
                {
                    var tmp = keyAccumulator.ToList();
                    keyAccumulator.Clear();
                    return tmp;
                },
                pollScheduler ?? TaskPoolScheduler.Default
                ));

            var logPolledSubscription = poller.State
                .Where(state => state != null)
                .Where(state => state.Count() > 0)
                .Subscribe(state =>
                {
                    actWithPolled(state);
                });

            cancelSource.Token.Register(() => transmittingSubscription?.Dispose());
            cancelSource.Token.Register(() => logPolledSubscription?.Dispose());

            _cancel = cancelSource;
        }
      
        public void Dispose()
        {
            _cancel?.Cancel();
        }

    }
}
