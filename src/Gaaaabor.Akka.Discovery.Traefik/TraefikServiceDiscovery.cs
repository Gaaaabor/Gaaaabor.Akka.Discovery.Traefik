using Akka.Actor;
using Akka.Configuration;
using Akka.Discovery;
using Akka.Event;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using static Gaaaabor.Akka.Discovery.Traefik.TraefikContracts;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public sealed class TraefikServiceDiscovery : ServiceDiscovery
    {
        private static readonly IDictionary<string, Func<List<string>, Service, bool>> _filterCache = BuildFilterCache();

        private readonly ExtendedActorSystem? _system;
        private readonly ILoggingAdapter? _logger;
        private readonly TraefikDiscoverySettings _traefikDiscoverySettings;

        public TraefikServiceDiscovery(ExtendedActorSystem? system)
        {
            _logger = Logging.GetLogger(system, typeof(TraefikServiceDiscovery));
            _system = system;

            _traefikDiscoverySettings = TraefikDiscoverySettings.Create(system?.Settings?.Config?.GetConfig("gaaaabor.akka.discovery.traefik"));
        }

        public override async Task<Resolved> Lookup(Lookup lookup, TimeSpan resolveTimeout)
        {
            var addresses = await GetAddressesAsync();

            var resolvedTargets = new List<ResolvedTarget>();
            if (_traefikDiscoverySettings?.Ports is null || !_traefikDiscoverySettings.Ports.Any())
            {
                return new Resolved(lookup.ServiceName, resolvedTargets);
            }

            foreach (var address in addresses)
            {
                // TODO: Implement!
                foreach (var port in _traefikDiscoverySettings.Ports)
                {
                    resolvedTargets.Add(new ResolvedTarget(host: address.ToString(), port: port, address: address));
                }
            }

            return new Resolved(lookup.ServiceName, resolvedTargets);
        }

        private async Task<IEnumerable<IPAddress>> GetAddressesAsync()
        {
            var addresses = new List<IPAddress>();

            try
            {
                _logger.Info("[TraefikServiceDiscovery] Getting addresses of Traefik services...");

                var endpoint = _traefikDiscoverySettings.Endpoint;
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    throw new Exception("Endpoint cannot be null or empty!");
                }

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(endpoint);
                using var httpResponseMessage = await httpClient.GetAsync("/api/http/services");
                var services = await httpResponseMessage.Content.ReadFromJsonAsync<List<Service>>();

                _logger.Info("[TraefikServiceDiscovery] Found services: {0}", services?.Count ?? 0);

                if (services is null)
                {
                    return addresses;
                }

                var filteredServices = _traefikDiscoverySettings.Filters?.Any() is true
                    ? ApplyFilters(services, _traefikDiscoverySettings.Filters)
                    : services;

                foreach (var service in filteredServices)
                {
                    var IpAddressWithPort = service.ServerStatus.FirstOrDefault(x => string.Equals(x.Value, "up", StringComparison.OrdinalIgnoreCase)).Key;
                    if (IpAddressWithPort is null)
                    {
                        continue;
                    }

                    var uri = new Uri(IpAddressWithPort);

                    _logger.Info("[TraefikServiceDiscovery] Found address {0}", uri);

                    if (IPAddress.TryParse(uri.Host, out var address) && !addresses.Contains(address))
                    {
                        addresses.Add(address);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[TraefikServiceDiscovery] Error during {0}.{1}: {2}", nameof(TraefikServiceDiscovery), nameof(GetAddressesAsync), ex.Message);
            }

            return addresses;
        }

        private IEnumerable<Service> ApplyFilters(List<Service> services, ImmutableList<Filter> filters)
        {
            foreach (var service in services)
            {
                foreach (var filter in filters)
                {
                    if (_filterCache.TryGetValue(filter.Name, out var filterFunc) && filterFunc(filter.Values, service))
                    {
                        yield return service;
                    }
                }
            }

            yield break;
        }

        internal static ImmutableList<Filter> ParseFiltersString(string? filtersString)
        {
            var filters = new List<Filter>();

            if (filtersString is null)
            {
                return filters.ToImmutableList();
            }

            var kvpList = filtersString.Split(';');
            foreach (var kvp in kvpList)
            {
                if (string.IsNullOrEmpty(kvp))
                    continue;

                var pair = kvp.Split('=');
                if (pair.Length != 2)
                    throw new ConfigurationException($"Failed to parse one of the key-value pairs in filters: {kvp}");

                filters.Add(new Filter(pair[0], pair[1].Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToList()));
            }

            return filters.ToImmutableList();
        }

        /// <summary>
        /// Keep it simple stupid filter cache creation.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Func<List<string>, Service, bool>> BuildFilterCache()
        {
            var dict = new Dictionary<string, Func<List<string>, Service, bool>>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(Service.Name), (values, root) => values.Contains(root.Name, StringComparer.OrdinalIgnoreCase) },
                { nameof(Service.Status), (values, root) => values.Contains(root.Status, StringComparer.OrdinalIgnoreCase)  },
                { nameof(Service.Provider), (values, root) => values.Contains(root.Provider, StringComparer.OrdinalIgnoreCase) },
                { nameof(Service.Type), (values, root) => values.Contains(root.Type, StringComparer.OrdinalIgnoreCase) },

                { nameof(Service.UsedBy), (values, root) => root.UsedBy != null && values.Intersect(root.UsedBy, StringComparer.OrdinalIgnoreCase).Any()  },

                { nameof(Service.LoadBalancer.PassHostHeader), (values, root) => root.LoadBalancer != null && values.Contains(root.LoadBalancer.PassHostHeader.ToString(), StringComparer.OrdinalIgnoreCase) },
                { nameof(Service.LoadBalancer.Servers), (values, root) => root.LoadBalancer != null && root.LoadBalancer?.Servers?.Any(x=> values.Contains(x.Url, StringComparer.OrdinalIgnoreCase)) is true },
                { nameof(Service.ServerStatus), (values, root) => root.ServerStatus != null && root.ServerStatus.Values.Intersect(values, StringComparer.OrdinalIgnoreCase).Any() },
            };

            return dict;
        }
    }

    public class Filter
    {
        public string Name { get; }
        public List<string> Values { get; } = new List<string>();

        public Filter(string name, List<string> values)
        {
            Name = name;
            Values = values;
        }

        public Filter(string name, string value)
        {
            Name = name;
            Values.Add(value);
        }
    }
}
