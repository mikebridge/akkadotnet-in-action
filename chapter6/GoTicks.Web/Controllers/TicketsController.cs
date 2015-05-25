using System;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using WebApp.Actors;
using WebApp.Actors.TicketProtocol;

namespace WebApp.Controllers
{
    [RoutePrefix("events/{event}/tickets")]
    public class TicketsController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> Post(String @event, TicketRequest ticketRequest)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Tickets>(new BoxOffice.GetTickets(@event, ticketRequest.Tickets));

            if (result.TicketList.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Created(Request.RequestUri, result);
            }
            
        }


        public class TicketRequest 
        {
            public int Tickets { get; set; }
        }
    }
}
