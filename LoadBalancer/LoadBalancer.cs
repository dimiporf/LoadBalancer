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
            // Define the port number for the load balancer
            int port = 80;
            // Specify the IP address to listen on (Any IP address)
            IPAddress ipAddress = IPAddress.Any;

            // Create a TCP listener on the specified IP address and port
            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start(); // Start listening for incoming TCP connections

            // Log that the load balancer has started
            Console.WriteLine($"Load balancer started on port {port}");

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
