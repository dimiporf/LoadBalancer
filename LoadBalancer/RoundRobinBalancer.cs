using System.Collections.Generic;

namespace LoadBalancer
{
    /// <summary>
    /// Class responsible for managing the round-robin load balancing algorithm.
    /// </summary>
    class RoundRobinLoadBalancer
    {
        // List of all backend servers
        public static readonly List<(string, int)> AllBackendServers = new List<(string, int)>
        {
            ("localhost", 8080),
            ("localhost", 8081),
            ("localhost", 8082)
        };

        // List of healthy backend servers
        private static List<(string, int)> healthyServers = new List<(string, int)>(AllBackendServers);
        private static int nextServerIndex = 0; // Index to keep track of the next server for round-robin

        /// <summary>
        /// Gets the next backend server using the round-robin algorithm.
        /// </summary>
        /// <returns>The next backend server.</returns>
        public static (string, int) GetNextBackendServer()
        {
            lock (healthyServers)
            {
                if (healthyServers.Count == 0)
                {
                    // If no servers are healthy, return a default server (could also throw an exception or return null)
                    return ("localhost", 8080);
                }

                // Get the next backend server using round-robin
                var backendServer = healthyServers[nextServerIndex];
                nextServerIndex = (nextServerIndex + 1) % healthyServers.Count; // Update the index for round-robin
                return backendServer;
            }
        }

        /// <summary>
        /// Sets the list of healthy backend servers.
        /// </summary>
        /// <param name="servers">The list of healthy servers.</param>
        public static void SetHealthyServers(List<(string, int)> servers)
        {
            lock (healthyServers)
            {
                healthyServers = new List<(string, int)>(servers);
            }
        }
    }
}
