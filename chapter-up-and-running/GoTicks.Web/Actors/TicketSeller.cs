using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace WebApp.Actors
{
    public class TicketSeller : ReceiveActor
    {
        private readonly string _event;

        public TicketSeller(String @event)
        {
            _event = @event;
            Initialize();
        }

        public readonly List<Ticket> _tickets = new List<Ticket>();

        #region Messages

        public class Ticket
        {
            public int Id { get; private set; }

            public Ticket(int id)
            {
                Id = id;
            }
        }

        public class Add
        {
            public IList<Ticket> NewTickets { get; private set; }

            public Add(IList<Ticket> newTickets)
            {
                NewTickets = newTickets;
            }
        }

        public class GetEvent
        {
        }

        #endregion

        
        public void Initialize()
        {
            Receive<Add>(message =>
            {
                _tickets.AddRange(message.NewTickets);
                
            });


            Receive<GetEvent>(message =>
                // TODO: "some"
                Context.Sender.Tell(new BoxOffice.Event(_event, _tickets.Count)));


//            Receive<Buy>(message =>
//            {
//                Console.WriteLine("TODO: Add Buy");
//            });



        }

    }
}