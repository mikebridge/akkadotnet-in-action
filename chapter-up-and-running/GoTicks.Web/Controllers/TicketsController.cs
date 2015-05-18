using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.Controllers
{
    [RoutePrefix("events/{event}/tickets")]
    public class TicketsController : ApiController
    {
        // GET: api/Tickets
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Tickets/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Tickets
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Tickets/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Tickets/5
        public void Delete(int id)
        {
        }
    }
}
