using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Akka.Management;
using Akka.Management.Cluster.Bootstrap;
using Akka.Remote.Hosting;
using Gaaaabor.Akka.Discovery.Traefik;
using System.Net;
using TraefikExample.Cluster;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAkka("weather", builder =>
{
    const int ManagementPort = 8558;
    const string Role = "WeatherForecast";
    const string TraefikEndpoint = "http://weather-traefik:8080";

    builder
        .WithRemoting(hostname: Dns.GetHostName(), port: 8091)
        .WithClustering(new ClusterOptions { Roles = new[] { Role } })
        .WithClusterBootstrap(serviceName: "exampleservice")
        .WithAkkaManagement(port: ManagementPort)
        .WithTraefikDiscovery(options =>
        {
            options.Endpoint = TraefikEndpoint;
            options.Ports = new List<int> { ManagementPort };
            options.Filters = new List<Filter>
            {
                new Filter("type", "loadbalancer"),
                new Filter("provider", "docker")
            };
        })
        .WithShardRegion<SimpleShardRegion>(nameof(SimpleShardRegion), SimpleShardRegion.ActorFactory, new SimpleMessageExtractor(), new ShardOptions
        {
            Role = Role
        })
        .WithActors((actorSystem, actorRegistry) =>
        {
            var simpleShardRegion = actorRegistry.Get<SimpleShardRegion>();
            simpleShardRegion.Tell(new ShardRegion.StartEntity("1"));
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
