using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Pattern;

namespace AIA.FaultTolerance
{
    public class LifeCycleHooks : ReceiveActor
    {
        private ILoggingAdapter _log;

        public LifeCycleHooks()
        {
            Console.WriteLine("Setting up stuff");
            _log = Logging.GetLogger(Context.System, this);

//            Receive<String>(s => s.Equals("restart"),
//                _ => ThrowIllegalStateException("force restart")
//                );

            ReceiveAny(msg => {
                                  Console.WriteLine("Receive");
                                  Sender.Tell(msg);
            });
        }

        protected override void PreStart()
        {
            base.PreStart();
            Console.WriteLine("PreStart");
        }

        protected override void PostStop()
        {
            base.PostStop();
            Console.WriteLine("PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
            Console.WriteLine("PreRestart");
        }

        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            Console.WriteLine("PostRestart");
        }

        private void ThrowIllegalStateException(string message)
        {
            throw new IllegalStateException(message);
        }
    }
}
