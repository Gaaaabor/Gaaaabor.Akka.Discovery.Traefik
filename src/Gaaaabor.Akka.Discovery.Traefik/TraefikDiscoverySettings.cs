using Akka.Actor;
using Akka.Configuration;
using System.Collections.Immutable;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public class TraefikDiscoverySettings
    {
        public static readonly TraefikDiscoverySettings Empty = new(
            ImmutableList<Filter>.Empty,
            ImmutableList<int>.Empty,
            null);

        public static TraefikDiscoverySettings Create(ActorSystem system) => Create(system.Settings.Config.GetConfig("akka.discovery.traefik"));

        public static TraefikDiscoverySettings Create(Config? config)
        {
            return new TraefikDiscoverySettings(
                TraefikServiceDiscovery.ParseFiltersString(config?.GetString("filters")),
                config?.GetIntList("ports").ToImmutableList(),
                config?.GetString("endpoint")
            );
        }

        public TraefikDiscoverySettings(
            ImmutableList<Filter>? filters,
            ImmutableList<int>? ports,
            string? endpoint)
        {
            Filters = filters;
            Ports = ports;
            Endpoint = endpoint;
        }

        public ImmutableList<Filter>? Filters { get; }
        public ImmutableList<int>? Ports { get; }
        public string? Endpoint { get; }


        public TraefikDiscoverySettings WithFilters(ImmutableList<Filter> filters) => Copy(filters: filters);

        public TraefikDiscoverySettings WithPorts(ImmutableList<int> ports) => Copy(ports: ports);

        public TraefikDiscoverySettings WithEndpoint(string endpoint) => Copy(endpoint: endpoint);

        private TraefikDiscoverySettings Copy(
            ImmutableList<Filter> filters = null,
            ImmutableList<int> ports = null,
            string endpoint = null)
            => new(
                filters: filters ?? Filters,
                ports: ports ?? Ports,
                endpoint: endpoint ?? Endpoint);
    }
}
