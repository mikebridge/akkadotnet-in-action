using System;
using System.Threading.Tasks;
using Akka.Actor;
using NUnit.Framework;

using Akka.TestKit.NUnit;

namespace AIA.Tests.TestDriven
{
    [TestFixture]
    public class EchoActorTest : TestKit
    {
        [TearDown]
        public void TearDown()
        {
            Shutdown();
        }

        [Test]
        public void An_EchoActor_Must_Reply_With_The_Same_Message_It_Receives()
        {
            var echo = Sys.ActorOf(Props.Create(() => new EchoActor()), "echo1");
            Task.Run(async () =>
            {
                await echo.Ask("some message");             
            }).ContinueWith(t =>
            {
                Console.WriteLine("Handle Faulted");
            }, TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t =>
            {
                Console.WriteLine("Handle Success");
            }, TaskContinuationOptions.OnlyOnRanToCompletion).Wait(TimeSpan.FromSeconds(3));
            Assert.Fail("Not sure what this test is supposed to do...?");
        }

        [Test]
        public void An_EchoActor_Must_Reply_With_The_Same_Message_It_Receives_Without_Ask()
        {
            var echo = Sys.ActorOf(Props.Create(() => new EchoActor()), "echo2");

            echo.Tell("some message");

            ExpectMsg("some message");
        }

    }
}

public class EchoActor : UntypedActor
{
    protected override void OnReceive(object message)
    {
        Context.Sender.Tell(message);
    }
}
