using System;

namespace LoadBalancer
{
    class RoundRobinLoadBalancer
    {
        // List of backend servers
        private static readonly (string, int)[] backendServers = {
            ("localhost", 8080),
            ("localhost", 8081),
            ("localhost", 8082)
        };

        private static int nextServerIndex = 0; // Index to keep track of the next server for round-robin

        public static (string, int) GetNextBackendServer()
        {
            // Get the next backend server using round-robin
            var backendServer = backendServers[nextServerIndex];
            nextServerIndex = (nextServerIndex + 1) % backendServers.Length; // Update the index for round-robin
            return backendServer;
        }
    }
}
