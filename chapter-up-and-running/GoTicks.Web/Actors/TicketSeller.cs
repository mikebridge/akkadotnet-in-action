using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace WebApp.Actors
{
    public class TicketSeller : ReceiveActor
    {
        private readonly string _event;

        public List<Ticket> _tickets = new List<Ticket>();

        public TicketSeller(String @event)
        {
            _event = @event;
            Initialize();
        }

        #region Messages

        public class Add
        {
            public IList<Ticket> NewTickets { get; private set; }

            public Add(IList<Ticket> newTickets)
            {
                NewTickets = newTickets;
            }
        }

        public class Buy
        {
            public int NrOfTickets { get; private set; }

            public Buy(int nrOfTickets)
            {
                NrOfTickets = nrOfTickets;
            }
        }

        public class Ticket
        {
            public int Id { get; private set; }

            public Ticket(int id)
            {
                Id = id;
            }
        }

        public class Tickets
        {
            public string Event { get; private set; }

            public IList<Ticket> Entries { get; private set; }

            public Tickets(String @event, IList<Ticket> entries = null)
            {
                Event = @event;
                Entries = entries ?? new List<Ticket>();
            }
        }

        public class Cancel {}

        public class GetEvent {}

        #endregion

        
        public void Initialize()
        {
            Receive<Add>(message =>
            {
                _tickets.AddRange(message.NewTickets);                
            });

            Receive<Buy>(message =>
            {
                var entries = _tickets.Take(message.NrOfTickets).ToList();

                if (entries.Count >= message.NrOfTickets)
                {
                    Context.Sender.Tell(new Tickets(_event, entries));
                    _tickets = _tickets.Skip(message.NrOfTickets).ToList();
                }
                else
                {
                    Context.Sender.Tell(new Tickets(_event));
                }
            });

            Receive<GetEvent>(message =>
                Context.Sender.Tell(new BoxOffice.Event(_event, _tickets.Count)));

            Receive<Cancel>(message =>
            {
                Context.Sender.Tell(new BoxOffice.Event(_event, _tickets.Count));                
                Self.Tell(PoisonPill.Instance);
            });

        }

    }
}