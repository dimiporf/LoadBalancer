using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LoadBalancer
{
    /// <summary>
    /// Class responsible for forwarding requests to backend servers.
    /// </summary>
    class BackendCommunicator
    {
        // Public connection pools for each backend server
        public static readonly ConcurrentDictionary<(string, int), ConcurrentBag<TcpClient>> ConnectionPools
            = new ConcurrentDictionary<(string, int), ConcurrentBag<TcpClient>>();

        /// <summary>
        /// Forwards the incoming request to a backend server using round-robin algorithm with Keep-Alive support.
        /// </summary>
        /// <param name="request">The request to be forwarded.</param>
        /// <returns>The response from the backend server.</returns>
        public static async Task<string> ForwardRequestToBackend(string request)
        {
            try
            {
                // Get the next backend server using the round-robin algorithm
                var backendServer = RoundRobinLoadBalancer.GetNextBackendServer();
                string host = backendServer.Item1;
                int port = backendServer.Item2;

                Console.WriteLine($"Selected backend server: {host}:{port}");

                // Get or create a connection pool for the backend server
                var connectionPool = ConnectionPools.GetOrAdd(backendServer, _ => new ConcurrentBag<TcpClient>());

                // Try to get an existing connection from the pool
                if (!connectionPool.TryTake(out TcpClient backendClient) || !backendClient.Connected)
                {
                    backendClient = new TcpClient();
                    await backendClient.ConnectAsync(host, port);
                    Console.WriteLine($"Established new connection to backend server {host}:{port}");
                }
                else
                {
                    Console.WriteLine($"Reusing existing connection to backend server {host}:{port}");
                }

                // Open streams for writing to and reading from the backend server
                using (NetworkStream stream = backendClient.GetStream())
                using (StreamWriter writer = new StreamWriter(stream))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Send the request to the backend server
                    Console.WriteLine("Sending request to backend server...");
                    await writer.WriteLineAsync(request);
                    await writer.FlushAsync();

                    // Wait for and read the response from the backend server
                    Console.WriteLine("Waiting for response from backend server...");
                    string response = await reader.ReadToEndAsync();

                    // Return the connection to the pool
                    connectionPool.Add(backendClient);
                    Console.WriteLine($"Returned connection to pool for backend server {host}:{port}");

                    return response;
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during communication with the backend server
                Console.WriteLine($"Error communicating with backend server: {ex.Message}");
                // Return an error response
                return "HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/plain\r\n\r\nError communicating with backend server";
            }
        }
    }
}