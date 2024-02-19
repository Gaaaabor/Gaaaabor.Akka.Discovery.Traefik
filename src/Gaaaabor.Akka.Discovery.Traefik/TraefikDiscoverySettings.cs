using Akka.Actor;
using Akka.Configuration;
using System.Collections.Immutable;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public class TraefikDiscoverySettings
    {
        public static readonly TraefikDiscoverySettings Empty = new(
            filters: ImmutableList<Filter>.Empty,
            ports: ImmutableList<int>.Empty,
            endpoint: null,
            auth: null);

        public static TraefikDiscoverySettings Create(ActorSystem system) => Create(system.Settings.Config.GetConfig("akka.discovery.traefik"));

        public static TraefikDiscoverySettings Create(Config config)
        {
            return new TraefikDiscoverySettings(
                TraefikServiceDiscovery.ParseFiltersString(config.GetString("filters")),
                config.GetIntList("ports").ToImmutableList(),
                config.GetString("endpoint"),
                TraefikApiAuth.Parse(config.GetString("auth"))
            );
        }

        public TraefikDiscoverySettings(
            ImmutableList<Filter> filters,
            ImmutableList<int> ports,
            string endpoint,
            TraefikApiAuth auth)
        {
            Filters = filters;
            Ports = ports;
            Endpoint = endpoint;
            Auth = auth;
        }

        public ImmutableList<Filter> Filters { get; }
        public ImmutableList<int> Ports { get; }
        public string Endpoint { get; }
        public TraefikApiAuth Auth { get; }

        public TraefikDiscoverySettings WithFilters(ImmutableList<Filter> filters) => Copy(filters: filters);

        public TraefikDiscoverySettings WithPorts(ImmutableList<int> ports) => Copy(ports: ports);

        public TraefikDiscoverySettings WithEndpoint(string endpoint) => Copy(endpoint: endpoint);

        public TraefikDiscoverySettings WithAuth(TraefikApiAuth auth) => Copy(auth: auth);

        private TraefikDiscoverySettings Copy(
            ImmutableList<Filter>? filters = null,
            ImmutableList<int>? ports = null,
            string? endpoint = null,
            TraefikApiAuth? auth = null)
            => new(
                filters: filters ?? Filters,
                ports: ports ?? Ports,
                endpoint: endpoint ?? Endpoint,
                auth: auth ?? Auth);
    }
}
