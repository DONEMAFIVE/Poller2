using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Xunit;

namespace PollerAppCore.Tests
{
    public class ImplementationTesting
    {
        [Fact]
        public void SimplePolling()
        {
            var sourcePollUnits = new List<TestPollerUnit>()
                                            {
                                                new TestPollerUnit()
                                                {
                                                    Timestamp = DateTime.Now,
                                                    Content= "unit_0"
                                                },
                                                new TestPollerUnit()
                                                {
                                                    Timestamp = DateTime.Now.AddMinutes(1),
                                                    Content= "unit_1"
                                                },
                                                new TestPollerUnit()
                                                {
                                                    Timestamp = DateTime.Now.AddMinutes(2),
                                                    Content= "unit_2"
                                                }
                                             };

            var pollEnumerator = sourcePollUnits.AsEnumerable().GetEnumerator();
            var testScheduler = new TestScheduler();

            var poller = (new PollerFactory<TestPollerUnit>().Create(
                            new PollerSettings() 
                            { 
                                Period = TimeSpan.FromSeconds(1) 
                            },
                            () =>
                            {
                                pollEnumerator.MoveNext();
                                var curr = pollEnumerator.Current;                                
                                return curr;
                            },
                            testScheduler));

            
            var pollResultsContainer = new List<String>();

            var pollerSubsciption = poller.State.Subscribe(state => pollResultsContainer.Add(state.Content));

            testScheduler.AdvanceTo(sourcePollUnits.Count() * poller.Settings.Period.Ticks);

            Assert.Collection(pollResultsContainer,
                                item => item.Equals("unit_0"),
                                item => item.Equals("unit_1"),
                                item => item.Equals("unit_2"));
        }
    }
}
