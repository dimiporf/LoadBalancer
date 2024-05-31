using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBalancer
{
    /// <summary>
    /// Class responsible for periodic health checks on backend servers.
    /// </summary>
    class HealthChecker
    {
        private static List<(string, int)> healthyServers = new List<(string, int)>();
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Starts periodic health checks on the backend servers.
        /// </summary>
        /// <param name="period">The interval between health checks, in seconds.</param>
        /// <param name="url">The health check URL to be appended to each server's address.</param>
        public static async Task StartHealthChecks(int period, string url)
        {
            while (true)
            {
                // Iterate through each backend server and perform health check
                foreach (var server in RoundRobinLoadBalancer.AllBackendServers)
                {
                    string healthCheckUrl = $"http://{server.Item1}:{server.Item2}{url}";
                    bool isHealthy = await CheckServerHealth(healthCheckUrl);
                    UpdateServerHealth(server, isHealthy);
                }

                // Delay before next round of health checks
                await Task.Delay(TimeSpan.FromSeconds(period));
            }
        }

        /// <summary>
        /// Performs a health check on the specified URL.
        /// </summary>
        /// <param name="url">The URL to perform the health check.</param>
        /// <returns>A boolean indicating whether the server is healthy or not.</returns>
        private static async Task<bool> CheckServerHealth(string url)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                // If an exception occurs, consider the server as unhealthy
                return false;
            }
        }

        /// <summary>
        /// Updates the list of healthy servers based on the health check results.
        /// </summary>
        /// <param name="server">The backend server to update.</param>
        /// <param name="isHealthy">A boolean indicating whether the server is healthy or not.</param>
        private static void UpdateServerHealth((string, int) server, bool isHealthy)
        {
            if (isHealthy && !healthyServers.Contains(server))
            {
                // If server is healthy and not in the list, add it
                healthyServers.Add(server);
                Console.WriteLine($"Server {server.Item1}:{server.Item2} is healthy and added to the list.");
            }
            else if (!isHealthy && healthyServers.Contains(server))
            {
                // If server is unhealthy and in the list, remove it
                healthyServers.Remove(server);
                Console.WriteLine($"Server {server.Item1}:{server.Item2} is unhealthy and removed from the list.");
            }

            // Update the list of healthy servers in the load balancer
            RoundRobinLoadBalancer.SetHealthyServers(healthyServers);
        }
    }
}