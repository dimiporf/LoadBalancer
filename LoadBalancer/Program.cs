using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Port to listen on
            int port = 80;

            // Start listening
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Load balancer started on port {port}");

            try
            {
                while (true)
                {
                    // Accept incoming connection
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    // Handle client request
                    HandleClientRequest(client);
                }
            }
            finally
            {
                // Stop listening
                listener.Stop();
            }
        }

        static async Task HandleClientRequest(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Read request
                string request = await reader.ReadLineAsync();
                Console.WriteLine($"Received request from {((IPEndPoint)client.Client.RemoteEndPoint).Address}:");
                Console.WriteLine(request);

                // Forward request to a single server (for now, replace with actual logic later)
                string forwardedResponse = await ForwardRequestToServer(request);

                // Send response back to client
                await writer.WriteLineAsync(forwardedResponse);
                await writer.FlushAsync();
            }

            // Close connection
            client.Close();
        }

        static async Task<string> ForwardRequestToServer(string request)
        {
            // In this basic example, just echo back the request
            return $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n{request}";
        }
    }
}
