using System;
using Akka.Actor;

namespace WebApp.Actors
{
    /// <summary>
    /// TODO: See if there's a better way to do this....
    /// </summary>
    public class SystemActors {
    
        public static IActorRef BoxOfficeActor = ActorRefs.Nobody;

        public static IActorRef CommandProcessor = ActorRefs.Nobody;
    }
}