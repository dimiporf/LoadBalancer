using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class HealthChecker
    {
        private static List<(string, int)> healthyServers = new List<(string, int)>();
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task StartHealthChecks(int period, string url)
        {
            while (true)
            {
                foreach (var server in RoundRobinLoadBalancer.AllBackendServers)
                {
                    string healthCheckUrl = $"http://{server.Item1}:{server.Item2}{url}";
                    bool isHealthy = await CheckServerHealth(healthCheckUrl);
                    UpdateServerHealth(server, isHealthy);
                }

                await Task.Delay(TimeSpan.FromSeconds(period));
            }
        }

        private static async Task<bool> CheckServerHealth(string url)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private static void UpdateServerHealth((string, int) server, bool isHealthy)
        {
            if (isHealthy && !healthyServers.Contains(server))
            {
                healthyServers.Add(server);
                Console.WriteLine($"Server {server.Item1}:{server.Item2} is healthy and added to the list.");
            }
            else if (!isHealthy && healthyServers.Contains(server))
            {
                healthyServers.Remove(server);
                Console.WriteLine($"Server {server.Item1}:{server.Item2} is unhealthy and removed from the list.");
            }

            RoundRobinLoadBalancer.SetHealthyServers(healthyServers);
        }
    }
}
