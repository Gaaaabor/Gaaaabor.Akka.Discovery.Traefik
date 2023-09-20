using Akka.Actor;
using TraefikExample.Actors;

namespace TraefikExample.Cluster
{
    public class SimpleShardRegion
    {
        public static Props ActorFactory(string entityId)
        {
            return Props.Create<WeatherForecastActor>(entityId);
        }
    }
}
