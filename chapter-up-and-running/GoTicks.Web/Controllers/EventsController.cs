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
        
        //public IEnumerable<string> Get()
        //{            
            //return new string[] { "value1", "value2" };
        //}
        /// https://github.com/akkadotnet/akka.net/issues/804
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
            return Ok(result);
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(NewEvent newEvent)
        {
            var result = await SystemActors.BoxOfficeActor.Ask<Object>(new BoxOffice.CreateEvent(newEvent.Name, newEvent.Tickets));
            return Ok(result);
        }

        // PUT api/events/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/events/5
        public void Delete(int id)
        {
        }
    }

    public class NewEvent
    {
        public String Name { get; set; }

        public int Tickets { get; set; }
    }
}

