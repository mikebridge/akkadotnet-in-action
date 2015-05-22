using System;

using Akka.Actor;
using Akka.Event;

namespace AIA.TestDriven
{
    public class Greeter : ReceiveActor
    {
        public class Greeting
        {
            public string Message { get; private set; }

            public Greeting(String message)
            {
                Message = message;
            }
        }

        public Greeter()
        {
            var log = Logging.GetLogger(Context.System, this);

            Receive<Greeting>(message => log.Info("Hello " + message.Message + "!"));
        }
    }
}
