namespace Gaaaabor.Akka.Discovery.Traefik
{
    public record Filter(string Name)
    {
        public List<string> Values { get; } = new List<string>();

        public Filter(string name, List<string> values) : this(name)
        {
            Values = values;
        }

        public Filter(string name, string value) : this(name)
        {
            Values.Add(value);
        }
    }
}
