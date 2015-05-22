using System;
using System.Collections.Generic;
using System.Linq;
using Agent01Protocol;

using Akka.Actor;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace AIA.Tests.TestDriven
{
    [TestFixture]
    public class SendingActor01Test : TestKit
    {
        [TestFixtureTearDown]
        public void TearDown()
        {
            Shutdown();
        }

        [Test]
        public void A_Sending_Actor_Must_Send_A_Message_To_An_Actor_When_It_Is_Finished()
        {
            var props = Props.Create(() => new Agent01(TestActor));
            var sendingActor = Sys.ActorOf(props, "Agent1");
            var tickets = new List<Ticket> {new Ticket(1), new Ticket(2), new Ticket(3)};
            var game = new Game("Lakers vs Bulls", tickets);

            sendingActor.Tell(game);

            ExpectMsg<Game>(g => g.Tickets.Count == tickets.Count() - 1);
        }

    }
}
namespace Agent01Protocol
{
    public class Ticket
    {
        private readonly int _seat;

        public Ticket(int seat)
        {
            _seat = seat;
        }
    }

    public class Game
    {
        public string Name { get; private set; }
        public IList<Ticket> Tickets { get; private set; }

        public Game(String name, IList<Ticket> tickets)
        {
            Name = name;
            Tickets = tickets;
        }
    }
}


public class Agent01 : ReceiveActor
{
    private readonly IActorRef _nextAgent;

    public Agent01(IActorRef nextAgent)
    {
        _nextAgent = nextAgent;
        Initialize();
    }

    private void Initialize()
    {
        Receive<Game>(game =>
        {
            var tail = game.Tickets.Skip(1).ToList();
            _nextAgent.Tell(new Game(game.Name, tail));

        });
    }
}

