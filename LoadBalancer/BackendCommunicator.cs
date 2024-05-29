using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class BackendCommunicator
    {
        public static async Task<string> ForwardRequestToBackend(string request)
        {
            try
            {
                // Get the next backend server using the round-robin algorithm
                var backendServer = RoundRobinLoadBalancer.GetNextBackendServer();
                string host = backendServer.Item1;
                int port = backendServer.Item2;

                // Create a TCP client to connect to the backend server
                using (TcpClient backendClient = new TcpClient())
                {
                    // Connect to the backend server asynchronously
                    Console.WriteLine($"Connecting to backend server {host}:{port}");
                    await backendClient.ConnectAsync(host, port);

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
                        return response;
                    }
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
