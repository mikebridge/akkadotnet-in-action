using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using WebApp.Actors;

namespace WebApp.Controllers
{
    [RoutePrefix("events")]
    public class EventsController : ApiController
    {
        // TODO: figure out error handling

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.GetEvents());
            return Ok(result);
        }

        [Route("{name}")]
        public async Task<IHttpActionResult> Get(String name)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.GetEvent(name));
            // TODO: implement the 404
            return Ok(result);
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(NewEvent newEvent)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.CreateEvent(newEvent.Name, newEvent.Tickets));
            return Ok(result);
        }

        [Route("{name}")]
        public async Task<IHttpActionResult> Delete(String name)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.CancelEvent(name));
            // TODO: Interpret the "null" result.
            return Ok(result);
        }
    }

    public class NewEvent
    {
        public String Name { get; set; }

        public int Tickets { get; set; }
    }
}

