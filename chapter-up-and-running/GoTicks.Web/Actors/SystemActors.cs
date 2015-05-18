using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;

namespace WebApp.Actors
{
    /// <summary>
    /// Todo: this doesn't seem right.
    /// </summary>
    public class SystemActors {
    
        public static IActorRef BoxOfficeActor = ActorRefs.Nobody;

        public static IActorRef CommandProcessor = ActorRefs.Nobody;
    }
}