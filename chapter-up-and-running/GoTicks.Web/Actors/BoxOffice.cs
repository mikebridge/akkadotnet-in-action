using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace WebApp.Actors
{
    public class BoxOffice : ReceiveActor
    {
        public BoxOffice()
        {
            Initialize();
        }

        #region Message types

        public class @Event
        {
            public String Name { get; private set; }
            public int Tickets { get; private set; }

            public @Event(String name, int tickets)
            {
                Name = name;
                Tickets = tickets;
            }

        }

        public class CreateEvent
        {
            public string Name { get; private set; }
            public int Tickets { get; private set; }

            public CreateEvent(String name, int tickets)
            {
                Name = name;
                Tickets = tickets;
            }
        }

        public class GetEvents
        {

        }

        public class GetEvent
        {
            public GetEvent(String name)
            {
                Name = name;
            }
            public String Name { get; private set; }
        }

        public class EventExists
        {            
        }

        public class EventCreated
        {
        }

        #endregion

        public void Initialize()
        {
            Receive<GetEvents>(_ =>
            {
                var evts = Context.GetChildren()
                    .Select(child => child.Ask<Object>(new TicketSeller.GetEvent()).Result);
  
                Context.Sender.Tell(evts);
                
            });

            Receive<GetEvent>(message =>
            {
                // TODO: IMplement this.
                Context.Sender.Tell("Test");
                //Context.Child(evt.Name)
                //def notFound() = sender() ! None
                //def getEvent(child: ActorRef) = child forward TicketSeller.GetEvent
                //context.child(event).fold(notFound())(getEvent)
            });

            Receive<CreateEvent>(message =>
            {
                //var childen = Context.GetChildren();

                var ticketSeller = Context.Child(message.Name).IsNobody()
                    ? CreateTicketSeller(message.Name)
                    : null; // the event exists
                if (ticketSeller == null)
                {
                    Context.Sender.Tell(new EventExists());
                    return;
                }
                
                // create ticket objects
                var newTickets = Enumerable.Range(1, 10).Select(ticketId => new TicketSeller.Ticket(ticketId)).ToList();
                

                // Tell the ticket seller about the new tickets
                ticketSeller.Tell(new TicketSeller.Add(newTickets));
                
                // Tell the sender that the event was created successfully
                Context.Sender.Tell(new EventCreated());
// def create() = {  //<co id="ch02_create"/>
//        val eventTickets = createTicketSeller(name)
//        val newTickets = (1 to tickets).map { ticketId =>
//          TicketSeller.Ticket(ticketId)
//        }.toVector
//        eventTickets ! TicketSeller.Add(newTickets)
//        sender() ! EventCreated
//      }
//      context.child(name).fold(create())(_ => sender() ! EventExists) //<co id="ch02_create_or_respond_with_exists"/>
//            });
            });
        }

        public IActorRef CreateTicketSeller(String name)
        {
            return Context.ActorOf(Props.Create(() => new TicketSeller(name)), name);
            //Context.ActorOf<TicketSeller>(Props.Create(() => new TicketSeller()), "TicketSeller");
            // def createTicketSeller(name:String) =
            // context.actorOf(TicketSeller.props(name), name) //<co id="ch02_create_ticket_seller"/>
        }
          


    }

}