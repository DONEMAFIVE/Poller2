using Microsoft.Reactive.Testing;
using Moq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Xunit;
using System.Linq;

namespace PollerAppCore.Tests
{
   
    public class IPollerTesting_moqOnly
    {
        [Fact]
        public void MockedPollerReturnSequence()
        {
            var testScheduler = new TestScheduler();

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

            var statesObservablePublished = sourcePollUnits
                                            .ToObservable()
                                            .ObserveOn(testScheduler);

            var poller = new Mock<IPoller<TestPollerUnit>>();
            poller.Setup(p => p.State).Returns(statesObservablePublished);

            var pollResultsContainer = new List<String>();
            var pollerInstance = poller.Object;
            var pollerSubsciption = pollerInstance.State.Subscribe(state => pollResultsContainer.Add(state.Content));

            testScheduler.AdvanceTo(sourcePollUnits.Count());

            Assert.Collection(pollResultsContainer,
                                item => item.Equals("unit_0"),
                                item => item.Equals("unit_1"),
                                item => item.Equals("unit_2"));
        }
    }
}
