using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AIA.FaultTolerance;
using Akka.Actor;
using Akka.Actor.Internals;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace AIA.Tests.FaultTolerance
{
    [TestFixture]
    public class LifeCycleHooksTest : TestKit
    {
        [TestFixtureTearDown]
        public void TearDown()
        {
            Shutdown();
        }

        [Test]
        public void The_Child_Must_Log_LifeCycle_Hooks()
        {
            
            var props = Props.Create(() => new LifeCycleHooks());
            var testActorRef = Sys.ActorOf(props, "LifeCycleHooks");

            testActorRef.Tell("restart");
            testActorRef.Tell("msg");
            Sys.Stop(testActorRef);
            Thread.Sleep(1000);
            Assert.Fail("This has no tests...");
        }
    }
}
