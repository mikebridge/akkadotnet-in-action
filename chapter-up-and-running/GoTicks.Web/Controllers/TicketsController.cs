using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using WebApp.Actors;

namespace WebApp.Controllers
{
    [RoutePrefix("events/{event}/tickets")]
    public class TicketsController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> Post(String @event, TicketRequest ticketRequest)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<TicketSeller.Tickets>(new BoxOffice.GetTickets(@event, ticketRequest.Tickets));

            if (result.Entries.Count == 0)
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
