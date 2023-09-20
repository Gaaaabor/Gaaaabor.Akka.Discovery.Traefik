using Akka.Actor;
using Akka.Configuration;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public class TraefikDiscovery : IExtension
    {
        public readonly TraefikDiscoverySettings Settings;

        public static Config DefaultConfiguration()
        {
            return ConfigurationFactory.FromObject(new TraefikServiceDiscoveryOptions());
        }

        public static TraefikDiscovery Get(ActorSystem system)
        {
            return system.WithExtension<TraefikDiscovery, TraefikDiscoveryProvider>();
        }

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
