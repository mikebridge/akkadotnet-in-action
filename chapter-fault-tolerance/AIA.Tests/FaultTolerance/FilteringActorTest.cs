using System.Collections.Generic;
using System.Linq;

using Akka.Actor;
using Akka.TestKit.NUnit;

using FilteringActorProtocol;
using NUnit.Framework;

namespace AIA.Tests.TestDriven
{
    [TestFixture]
    public class FilteringActorTest : TestKit
    {
        [TestFixtureTearDown]
        public void TearDown()
        {
            Shutdown();
        }

        
        [Test]
        public void A_Filtering_Actor_Must_Filter_Out_Particular_Messages()
        {
            var props = Props.Create(() => new FilteringActor(TestActor, 5));
            var filter = Sys.ActorOf(props, "filter-1");

            new List<int>{1, 2, 1, 3, 1, 4, 5, 5, 6}.ForEach(x => filter.Tell(new Event(x)));

            var eventIds = ReceiveWhile<Event>(x => x.Id <= 5);

            Assert.That(eventIds.Select(x => x.Id), Is.EquivalentTo(new List<int> {1, 2, 3, 4, 5}));
            ExpectMsg<Event>(x => x.Id == 6);

        }

        [Test]
        public void A_Filtering_Actor_Must_Filter_Out_Particular_Messages_Using_ExpectNoMsg()
        {
            var props = Props.Create(() => new FilteringActor(TestActor, 5));
            var filter = Sys.ActorOf(props, "filter-2");

            new List<int> { 1, 2 }.ForEach(x => filter.Tell(new Event(x)));
            ExpectMsg<Event>(x => x.Id == 1);
            ExpectMsg<Event>(x => x.Id == 2);

            filter.Tell(new Event(1));
            ExpectNoMsg();
            
            filter.Tell(new Event(3));
            ExpectMsg<Event>(x => x.Id == 3);

            filter.Tell(new Event(1));
            ExpectNoMsg();

            new List<int> { 4, 5, 5 }.ForEach(x => filter.Tell(new Event(x)));
            ExpectMsg<Event>(x => x.Id == 4);
            ExpectMsg<Event>(x => x.Id == 5);
            ExpectNoMsg();

        }
    }
}

namespace FilteringActorProtocol
{
    public class Event
    {
        public long Id { get; private set; }

        public Event(long id)
        {
            Id = id;
        }
    }
}

public class FilteringActor : ReceiveActor
{
    public IActorRef NextActor { get; private set; }
    public int BufferSize { get; private set; }
    private IList<Event> _lastMessages = new List<Event>();

    public FilteringActor(IActorRef nextActor, int bufferSize)
    {
        NextActor = nextActor;
        BufferSize = bufferSize;
        Initialize();
    }

    private void Initialize()
    {
        Receive<Event>(message =>
        {
            if (_lastMessages.All(x => x.Id != message.Id))
            {
                _lastMessages.Add(message);
                NextActor.Tell(message);
                if (_lastMessages.Count > BufferSize)
                {
                    // discard the oldest
                    _lastMessages = _lastMessages.Skip(1).ToList();
                }
            }
        });
    }
}

