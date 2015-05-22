using System;
using System.Collections.Generic;

using Akka.Actor;
using Akka.TestKit.NUnit;

using NUnit.Framework;

namespace AIA.Tests.TestDriven
{
    [TestFixture]
    public class SilentActorNextTest : TestKit
    {
        [TestFixtureTearDown]
        public void TearDown()
        {
            Shutdown();
        }

        [Test]
        public void A_Silent_Actor_Must_Change_State_When_It_Receives_A_Message_Single()
        {
            var silentActor = ActorOfAsTestActorRef<SilentActor>();

            silentActor.Tell(new SilentActorProtocol.SilentMessage("whisper"));

            Assert.That(silentActor.UnderlyingActor.State, Has.Member("whisper"));

        }

        [Test]
        public void A_Silent_Actor_Must_Change_State_When_It_Receives_A_Message_Multi()
        {
            var silentActor = Sys.ActorOf(Props.Create(() => new SilentActor()), "s3");

            silentActor.Tell(new SilentActorProtocol.SilentMessage("whisper1"));
            silentActor.Tell(new SilentActorProtocol.SilentMessage("whisper2"));
            silentActor.Tell(new SilentActorProtocol.GetState(TestActor));

            ExpectMsg(new List<String> {"whisper1", "whisper2"});
        }


    }

    namespace SilentActorProtocol
    {
        public class SilentMessage
        {
            public string Data { get; private set; }

            public SilentMessage(String data)
            {
                Data = data;
            }
        }
        public class GetState
        {
            public IActorRef Receiver { get; private set; }

            public GetState(IActorRef receiver)
            {
                Receiver = receiver;
            }
        }

    }

    public class SilentActor: ReceiveActor
    {

        private readonly IList<String> _internalState = new List<String>();

        public SilentActor()
        {
            Initialize();
        }

        private void Initialize()
        {
            Receive<SilentActorProtocol.SilentMessage>(message =>
            {
                _internalState.Add(message.Data);
            });

            Receive<SilentActorProtocol.GetState>(message =>
            {
                message.Receiver.Tell(_internalState);

            });

        }

        public IList<String> State { get { return _internalState;} }

    }

}
