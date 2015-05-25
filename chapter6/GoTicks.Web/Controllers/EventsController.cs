using System;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using WebApp.Actors;

namespace WebApp.Controllers
{
    [RoutePrefix("events")]
    public class EventsController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.GetEvents());
            return Ok(result);
        }

        [Route("{name}")]
        public async Task<IHttpActionResult> Get(String name)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<BoxOffice.Event>(new BoxOffice.GetEvent(name));
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(NewEvent newEvent)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<BoxOffice.IEventResponse>(new BoxOffice.CreateEvent(newEvent.Name, newEvent.Tickets));
            if (result is BoxOffice.EventExists)
            {
                return BadRequest(newEvent.Name + " event exists already.");
            }
            else
            {
                return Created(Request.RequestUri + newEvent.Name, "Created");
            }
        }

        [Route("{name}")]
        public async Task<IHttpActionResult> Delete(String name)
        {
            await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.CancelEvent(name));
            return Ok();
        }
    }

    public class NewEvent
    {
        public String Name { get; set; }

        public int Tickets { get; set; }
    }
}

