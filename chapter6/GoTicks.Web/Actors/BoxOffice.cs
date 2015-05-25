using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Akka.Actor;
using Akka.Event;
using WebApp.Actors.TicketProtocol;
using WebApp.Controllers;

namespace WebApp.Actors
{
    public class BoxOffice : ReceiveActor
    {
        // TODO: timeout?
        ILoggingAdapter _log = Logging.GetLogger(Context.System, this);

        public BoxOffice()
        {
            Initialize();
        }

        private void Initialize()
        {
            Receive<Event>(message =>
            {                
                _log.Info("Creating new event "+ message.Name +" with "+message.NrOfTickets +" tickets.");
                var ticketSeller = Context.Child(message.Name).IsNobody()
                    ? CreateTicketSeller(message.Name)
                    : null; // the event exists

                if (ticketSeller == null)
                {
                    Context.Sender.Tell(new EventExists());
                    return;
                }

                // create ticket objects
                var newTickets = Enumerable.Range(1, message.Tickets).Select(ticketId => new TicketSeller.Ticket(ticketId)).ToList();

                // Tell the ticket seller about the new tickets
                ticketSeller.Tell(new TicketSeller.Add(newTickets));

                // Tell the sender that the event was created successfully
                Context.Sender.Tell(new EventCreated());
            });

            Receive<TicketRequest>(message =>
            {
                _log.Info("Getting a ticket for the " + message.EventName + " event.");
                // TODO
            });

            Receive<GetEvents>(message =>
            {
                var capturedSender = Sender;
                // TODO
            });

        }

        public IActorRef CreateTicketSeller(String name)
        {
            return Context.ActorOf(Props.Create(() => new TicketSeller(name)), name);
        }

//        #region Message types

//        public class CreateEvent
//        {
//            public string Name { get; private set; }
//            public int Tickets { get; private set; }
//
//            public CreateEvent(String name, int tickets)
//            {
//                Name = name;
//                Tickets = tickets;
//            }
//        }
//
//        public class GetEvent
//        {
//            public GetEvent(String name)
//            {
//                Name = name;
//            }
//
//            public String Name { get; private set; }
//        }
//
//        public class GetEvents {}
//
//        public class @Event
//        {
//            public String Name { get; private set; }
//            public int Tickets { get; private set; }
//
//            public @Event(String name, int tickets)
//            {
//                Name = name;
//                Tickets = tickets;
//            }
//
//        }
//
//        public interface IEventResponse
//        {
//        }
//
//        public class EventExists : IEventResponse
//        {
//        }
//
//        public class EventCreated : IEventResponse
//        {
//        }
//
//        public class CancelEvent
//        {
//            public string Name { get; private set; }
//
//            public CancelEvent(String name)
//            {
//                Name = name;
//            }
//        }
//
//        public class GetTickets
//        {
//            public string Name { get; private set; }
//            public int Tickets { get; private set; }
//
//            public GetTickets(String name, int tickets)
//            {
//                Name = name;
//                Tickets = tickets;
//            }
//        }
//
//
//        #endregion
//
//        public void Initialize()
//        {
//            Receive<CreateEvent>(message =>
//            {
//                //var childen = Context.GetChildren();
//                _log.Info("Creating new event "+ message.Name +" with "+message.Tickets +" tickets.");
//                var ticketSeller = Context.Child(message.Name).IsNobody()
//                    ? CreateTicketSeller(message.Name)
//                    : null; // the event exists
//
//                if (ticketSeller == null)
//                {
//                    Context.Sender.Tell(new EventExists());
//                    return;
//                }
//
//                // create ticket objects
//                var newTickets = Enumerable.Range(1, message.Tickets).Select(ticketId => new TicketSeller.Ticket(ticketId)).ToList();
//
//                // Tell the ticket seller about the new tickets
//                ticketSeller.Tell(new TicketSeller.Add(newTickets));
//
//                // Tell the sender that the event was created successfully
//                Context.Sender.Tell(new EventCreated());
//
//            });
//
//            Receive<TicketsController.TicketRequest>(message =>
//                _log.Info("Getting a ticket for the "+message.Name + "event")
//
//                );
//
//
//            Receive<GetEvent>(message =>
//            {
//                var childRef = Context.Child(message.Name);
//                if (childRef.IsNobody())
//                {
//                    Sender.Tell(null); // should this be a EventNotFound message?
//                }
//                else
//                {
//                    
//                    childRef.Forward(new TicketSeller.GetEvent());
//                }
//            });
//
//            Receive<GetEvents>(_ =>
//            {
//                var evts = Context.GetChildren()
//                    .Select(child => child.Ask<Object>(new TicketSeller.GetEvent()).Result);
//                Context.Sender.Tell(evts);
//
//            });
//
//            Receive<GetTickets>(message =>
//            {
//                var childRef = Context.Child(message.Name);
//                if (childRef.IsNobody())
//                {
//                    Context.Sender.Tell(new TicketSeller.Tickets(message.Name));
//                }
//                else
//                {
//                    childRef.Forward(new TicketSeller.Buy(message.Tickets));
//                }
//
//            });
//
//
//            Receive<CancelEvent>(message =>
//            {
//                var actorRef = Context.Child(message.Name);
//                if (actorRef.IsNobody())
//                {
//                    // TODO: Should this return null?
//                    Context.Sender.Tell(null);
//                }
//                else
//                {
//                    actorRef.Forward(new TicketSeller.Cancel());
//                }
//            });
//        }
//
//        public IActorRef CreateTicketSeller(String name)
//        {
//            return Context.ActorOf(Props.Create(() => new TicketSeller(name)), name);
//        }


    }
}