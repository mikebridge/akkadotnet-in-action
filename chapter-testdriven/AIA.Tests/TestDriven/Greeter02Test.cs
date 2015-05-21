using System;

using AIA.TestDriven;
using Akka.Actor;
using Akka.Event;

using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace AIA.Tests.TestDriven
{
    [TestFixture]
    public class Greeter02Test : TestKit
    {

        [TestFixtureTearDown]
        public void TearDown()
        {
            Shutdown();
        }


        [Test]
        public void The_Greeter_Must_Say_Hello_World_When_a_Greeting_WORLD_is_Sent_To_It()
        {
            var props = Props.Create(() => new Greeter02(TestActor));
            var greeter = Sys.ActorOf(props, "greeter02-1");
            
            greeter.Tell(new Greeter.Greeting("World"));

            ExpectMsg("Hello World!");
        }

        [Test]
        public void The_Greeter_Must_Say_Something_Else_And_See_What_Happens()
        {
            var props = Props.Create(() => new Greeter02(TestActor));
            var greeter = Sys.ActorOf(props, "greeter02-2");
            Sys.EventStream.Subscribe(TestActor, typeof (UnhandledMessage));

            greeter.Tell("World");

            ExpectMsg<UnhandledMessage>(msg =>
                ((String) msg.Message) == "World"
                        //&& msg.Sender.Equals(Sys.DeadLetters) // Different result?
                        && msg.Sender.Equals(TestActor)
                        && msg.Recipient.Equals(greeter)
            );
        }

    }
}

public class Greeter02 : ReceiveActor
{
    private readonly IActorRef _listener;
    private readonly ILoggingAdapter _log;

    public Greeter02(IActorRef listener = null)
    {
        _listener = listener;
        _log =  Logging.GetLogger(Context.System, this);

        Receive<Greeter.Greeting>(who =>
        {
            var message = "Hello " + who.Message + "!";
            _log.Info(message);
            if (_listener != null)
            {
                _listener.Tell(message);
            }
        });
    }
}
