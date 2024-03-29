# Sharding example with Traefik

The docker-compose.yml contains a traefik service and the example project with 5 instance.

Before running set the "GeneratePackageOnBuild" to "False" in Gaaaabor.Akka.Discovery.Traefik.csproj ("<GeneratePackageOnBuild>False</GeneratePackageOnBuild>" )

The cluster starts with 5 node by default, the example can be started using the following UP command:
docker compose -f PATH_TO_THE_REPO/Gaaaabor.Akka.Discovery.Traefik\docker-compose.yml up --force-recreate --build

...and can be shut down with the following DOWN command:
docker compose -f PATH_TO_THE_REPO\Gaaaabor.Akka.Discovery.Traefik\docker-compose.yml down

The example controller can be reached on the http://localhost/WeatherForecast endpoint.
The Traefik dashboard can be reached on the http://localhost:9999/endpoint. (User: admin, Pwd: password)
The service discovery uses the http://weather-traefik:8080/api/http/services endpoint to get the running services using basic auth header.

For more info on Traefik check the project's site here https://doc.traefik.io/traefik/
For more info in Akka Cluster check the project's site here https://getakka.net/articles/clustering/cluster-overview.html