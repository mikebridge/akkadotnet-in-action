using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Akka.Actor;
using Akka.Routing;
using WebApp.Actors;

namespace WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : HttpApplication
    {
        protected static ActorSystem ActorSystem;

        protected void Application_Start()
        {
           
            GlobalConfiguration.Configure(WebApiConfig.Register);
          
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            
            // see: https://github.com/petabridge/akkadotnet-code-samples/blob/master/Cluster.WebCrawler/README.md
            // also: http://stackoverflow.com/questions/27634843/akka-net-actor-system-in-asp-net
            ActorSystem = ActorSystem.Create("goticks");
            //var router = ActorSystem.ActorOf(Props.Create(() => new RemoteJobActor()).WithRouter(FromConfig.Instance), "tasker");
            //SystemActors.CommandProcessor = ActorSystem.ActorOf(Props.Create(() => new CommandProcessor(router)),
            //    "commands");
            //SystemActors.SignalRActor = ActorSystem.ActorOf(Props.Create(() => new SignalRActor()), "signalr");

            // TODO: In the example, the RestApi is the toplevel actor
            SystemActors.BoxOfficeActor = ActorSystem.ActorOf(Props.Create(() => new BoxOffice()), "box-office");

        }
    }
}