using Akka.Actor;
using Akka.Configuration;

namespace Akka.Discovery.Traefik
{
    public class TraefikDiscovery : IExtension
    {
        public static Configuration.Config DefaultConfiguration()
        {
            return ConfigurationFactory.FromResource<TraefikDiscovery>("Akka.Discovery.Traefik.reference.conf");

            //return ConfigurationFactory.FromObject(new TraefikServiceDiscoveryOptions());
        }

        public static TraefikDiscovery Get(ActorSystem system) => system.WithExtension<TraefikDiscovery, TraefikDiscoveryProvider>();

        public readonly TraefikDiscoverySettings Settings;

        public TraefikDiscovery(ExtendedActorSystem system)
        {
            system.Settings.InjectTopLevelFallback(DefaultConfiguration());
            Settings = TraefikDiscoverySettings.Create(system);

            var setup = system.Settings.Setup.Get<TraefikDiscoverySetup>();
            if (setup.HasValue)
            {
                Settings = setup.Value.Apply(Settings);
            }
        }
    }

    public class TraefikDiscoveryProvider : ExtensionIdProvider<TraefikDiscovery>
    {
        public override TraefikDiscovery CreateExtension(ExtendedActorSystem system) => new TraefikDiscovery(system);
    }
}
