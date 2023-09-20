using Akka.Actor.Setup;
using Akka.Hosting;
using System.Text;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public class TraefikServiceDiscoveryOptions : IHoconOption
    {
        private const string FullPath = "akka.discovery.traefik";

        public string ConfigPath { get; } = "traefik";

        public Type Class { get; } = typeof(TraefikServiceDiscovery);

        /// <summary>
        ///     Additional filtering rules to be applied to the possible Traefik contact points
        /// </summary>
        public List<Filter>? Filters { get; set; } = new();

        /// <summary>
        ///     List of ports to be considered as Akka.Management ports on each instance.
        /// </summary>
        public List<int>? Ports { get; set; } = new();

        /// <summary>
        /// <para>
        ///     Client may use specified endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        public void Apply(AkkaConfigurationBuilder builder, Setup setup = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{FullPath} {{");
            sb.AppendLine($"class = {Class.AssemblyQualifiedName!.ToHocon()}");

            if (Filters is { })
            {
                var filters = Filters
                    .SelectMany(filter => filter.Values.Select(value => (filter.Name, Tag: value)))
                    .Select(t => $"{t.Name}={t.Tag}");

                sb.AppendLine($"filters = {string.Join(";", filters).ToHocon()}");
            }

            if (Ports is { })
            {
                sb.AppendLine($"ports = [{string.Join(",", Ports)}]");
            }

            if (Endpoint is { })
            {
                sb.AppendLine($"endpoint = {Endpoint.ToHocon()}");
            }

            sb.AppendLine("}");

            builder.AddHocon(sb.ToString(), HoconAddMode.Prepend);
        }
    }
}
