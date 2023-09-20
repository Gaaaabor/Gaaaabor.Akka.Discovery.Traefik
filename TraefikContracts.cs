namespace Akka.Discovery.Traefik
{
    public class TraefikContracts
    {
        public class LoadBalancer
        {
            public List<Server> Servers { get; set; }
            public bool PassHostHeader { get; set; }
        }

        public class Service
        {
            public string Status { get; set; }
            public List<string> UsedBy { get; set; }
            public string Name { get; set; }
            public string Provider { get; set; }
            public LoadBalancer LoadBalancer { get; set; }
            public Dictionary<string, string> ServerStatus { get; set; }
            public string Type { get; set; }
        }

        public class Server
        {
            public string Url { get; set; }
        }
    }
}
