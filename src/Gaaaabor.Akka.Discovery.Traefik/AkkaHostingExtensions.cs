using Akka.Hosting;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public static class AkkaHostingExtensions
    {
        /// <summary>
        ///     Adds Akka.Discovery.Traefik support to the <see cref="ActorSystem"/>.
        ///     Note that this only adds the discovery plugin, you will still need to add ClusterBootstrap for
        ///     a complete solution.
        /// </summary>
        /// <param name="builder">
        ///     The builder instance being configured.
        /// </param>
        /// <returns>
        ///     The same <see cref="AkkaConfigurationBuilder"/> instance originally passed in.
        /// </returns>
        /// <example>
        ///   <code>
        ///     services.AddAkka("mySystem", builder => {
        ///         builder
        ///             .WithClustering()
        ///             .WithClusterBootstrap(options =>
        ///             {
        ///                 options.ContactPointDiscovery.ServiceName = "testService";
        ///                 options.ContactPointDiscovery.RequiredContactPointsNr = 1;
        ///             }, autoStart: true)
        ///             .WithTraefikDiscovery();
        ///     }
        ///   </code>
        /// </example>
        public static AkkaConfigurationBuilder WithTraefikDiscovery(this AkkaConfigurationBuilder builder) => builder.WithTraefikDiscovery(new TraefikServiceDiscoveryOptions());

        /// <summary>
        ///     Adds Akka.Discovery.Traefik support to the <see cref="ActorSystem"/>.
        ///     Note that this only adds the discovery plugin, you will still need to add ClusterBootstrap for
        ///     a complete solution.
        /// </summary>
        /// <param name="builder">
        ///     The builder instance being configured.
        /// </param>
        /// <param name="configure">
        ///     An action that modifies an <see cref="TraefikDiscoverySetup"/> instance, used
        ///     to configure Akka.Discovery.Traefik.
        /// </param>
        /// <returns>
        ///     The same <see cref="AkkaConfigurationBuilder"/> instance originally passed in.
        /// </returns>
        /// <example>
        ///   <code>
        ///     services.AddAkka("mySystem", builder => {
        ///         builder
        ///             .WithClustering()
        ///             .WithClusterBootstrap(options =>
        ///             {
        ///                 options.ContactPointDiscovery.ServiceName = "testService";
        ///                 options.ContactPointDiscovery.RequiredContactPointsNr = 1;
        ///             }, autoStart: true)
        ///             .WithTraefikDiscovery(options => {
        ///                 options.Endpoint = "http://traefik:8080";
        ///                 options.Ports = new List<int> { managementPort };
        ///             });
        ///     }
        ///   </code>
        /// </example>
        public static AkkaConfigurationBuilder WithTraefikDiscovery(this AkkaConfigurationBuilder builder, Action<TraefikServiceDiscoveryOptions> configure)
        {
            var options = new TraefikServiceDiscoveryOptions();
            configure(options);
            return builder.WithTraefikDiscovery(options);
        }

        /// <summary>
        ///     Adds Akka.Discovery.Traefik support to the <see cref="ActorSystem"/>.
        ///     Note that this only adds the discovery plugin, you will still need to add ClusterBootstrap for
        ///     a complete solution.
        /// </summary>
        /// <param name="builder">
        ///     The builder instance being configured.
        /// </param>
        /// <param name="options">
        ///     The <see cref="TraefikDiscoverySetup"/> instance used to configure Akka.Discovery.Traefik.
        /// </param>
        /// <returns>
        ///     The same <see cref="AkkaConfigurationBuilder"/> instance originally passed in.
        /// </returns>
        /// <example>
        ///   <code>
        ///     services.AddAkka("mySystem", builder => {
        ///         builder
        ///             .WithClustering()
        ///             .WithClusterBootstrap(options =>
        ///             {
        ///                 options.ContactPointDiscovery.ServiceName = "testService";
        ///                 options.ContactPointDiscovery.RequiredContactPointsNr = 1;
        ///             }, autoStart: true)
        ///             .WithTraefikDiscovery(new TraefikServiceDiscoveryOptions {
        ///                 TagKey = "myTag"
        ///             });
        ///     }
        ///   </code>
        /// </example>
        public static AkkaConfigurationBuilder WithTraefikDiscovery(this AkkaConfigurationBuilder builder, TraefikServiceDiscoveryOptions options)
        {
            builder.AddHocon($"akka.discovery.method = {options.ConfigPath}", HoconAddMode.Prepend);
            options.Apply(builder);
            builder.AddHocon(TraefikDiscovery.DefaultConfiguration(), HoconAddMode.Append);

            // force start the module
            builder.AddStartup((system, registry) =>
            {
                TraefikDiscovery.Get(system);
            });
            return builder;
        }
    }
}
