﻿using Akka.Actor.Setup;
using System.Collections.Immutable;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    internal class TraefikDiscoverySetup : Setup
    {
        /// <summary>
        /// Additional filtering rules to be applied to the possible Traefik contact points
        /// </summary>
        public List<Filter>? Filters { get; set; }

        /// <summary>
        /// List of ports to be considered as Akka.Management ports on each instance.
        /// Use this if you have multiple Akka.NET nodes per Traefik instance
        /// </summary>
        public List<int>? Ports { get; set; }

        /// <summary>        
        /// Client may use specified endpoint.
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// Client may use Basic Auth with User:Password base64 in Authorization header
        /// </summary>
        public TraefikApiAuth? Auth { get; set; }

        internal TraefikDiscoverySettings Apply(TraefikDiscoverySettings settings)
        {
            if (Filters != null)
            {
                settings = settings.WithFilters(Filters.ToImmutableList());
            }

            if (Ports != null)
            {
                settings = settings.WithPorts(Ports.ToImmutableList());
            }

            if (Endpoint != null)
            {
                settings = settings.WithEndpoint(Endpoint);
            }

            if (Auth != null)
            {
                settings = settings.WithAuth(Auth);
            }

            return settings;
        }
    }
}
