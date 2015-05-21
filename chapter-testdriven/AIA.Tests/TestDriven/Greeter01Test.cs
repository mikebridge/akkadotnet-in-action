using AIA.TestDriven;
using Akka.Actor;

using Akka.TestKit;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace AIA.Tests.TestDriven
{
    [TestFixture]
    public class Greeter01Test : TestKit
    {

        [TestFixtureTearDown]
        public void TearDown()
        {
            Shutdown();
        }


        [Test]
        public void The_Greeter_Must_Say_Hello_World_When_a_Greeting_WORLD_is_Sent_To_It()
        {
            // TestEventListener is the default test logger:
            // https://github.com/akkadotnet/akka.net/issues/323
            
            var dispatcherId = CallingThreadDispatcher.Id;
            var props = Props.Create(() => new Greeter()).WithDispatcher(dispatcherId);
            var greeter = Sys.ActorOf(props);
            EventFilter.Info(message: "Hello World!").ExpectOne(() => greeter.Tell(new Greeter.Greeting("World")));

        }
    }
}
