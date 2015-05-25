using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Akka.Actor;
using WebApp.Actors.TicketProtocol;

namespace WebApp.Actors
{
    public class TicketSeller : ReceiveActor
    {
        private List<Ticket> _tickets = new List<Ticket>();

        public TicketSeller()
        {
            Receive<GetEvents>(message =>
            {
                Context.Sender.Tell(new SoldOut());
                Self.Tell(PoisonPill.Instance);
            });

            Receive<Tickets>(message =>
            {
                _tickets.AddRange(message.TicketList);
            });

            Receive<BuyTicket>(message =>
            {
                if (_tickets.Any())
                {
                    Context.Sender.Tell(new SoldOut());
                    Self.Tell(PoisonPill.Instance);
                    return;
                }

                Context.Sender.Tell(_tickets.Take(1));
                _tickets = _tickets.Skip(1).ToList();
            });
            
        }

    }



    namespace TicketProtocol
    {
        public class Event {
            public string Name { get; private set; }
            public int NrOfTickets { get; private set; }

            public Event(String name, int nrOfTickets)
            {
                Name = name;
                NrOfTickets = nrOfTickets;
            }
        }

        public class GetEvents { }

        public class Events
        {
            public IList<Event> EventList { get; private set; }

            public Events(IList<Event> eventlist)
            {
                EventList = eventlist;
            }
        }

        public class EventCreated { }

        public class TicketRequest{

            public string EventName { get; private set; }

            public TicketRequest(String eventName)
            {
                EventName = eventName;
            }
        }

        public class SoldOut { }

        public class Tickets
        {
            public IList<Ticket> TicketList { get; private set; }
            public Tickets(IList<Ticket> tickets)
            {
                TicketList = tickets;
            }
        }

        public class BuyTicket { }

        public class Ticket
        {
            public string EventName { get; private set; }
            public int Nr { get; private set; }

            public Ticket(String eventName, int nr)
            {
                EventName = eventName;
                Nr = nr;
            }
        }

    }

}