using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class LoadBalancer
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: LoadBalancer <healthCheckPeriodInSeconds> <healthCheckUrl>");
                return;
            }

            if (!int.TryParse(args[0], out int healthCheckPeriod))
            {
                Console.WriteLine("Invalid health check period");
                return;
            }

            string healthCheckUrl = args[1];

            // Define the port number for the load balancer
            int port = 80;
            // Specify the IP address to listen on (Any IP address)
            IPAddress ipAddress = IPAddress.Any;

            // Create a TCP listener on the specified IP address and port
            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start(); // Start listening for incoming TCP connections

            // Log that the load balancer has started
            Console.WriteLine($"Load balancer started on port {port}");

            // Start health check task
            _ = HealthChecker.StartHealthChecks(healthCheckPeriod, healthCheckUrl);

            // Accept and handle incoming client connections in a loop
            while (true)
            {
                Console.WriteLine("Waiting for incoming client connection...");
                TcpClient client = await listener.AcceptTcpClientAsync(); // Accept incoming client connection
                Console.WriteLine("Client connected!");
                ClientHandler.HandleClient(client); // Handle the client connection asynchronously
            }
        }
    }
}
