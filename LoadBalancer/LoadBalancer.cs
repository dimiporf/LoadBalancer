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
        // List of backend servers
        private static readonly string[] backendHosts = { "localhost:8080", "localhost:8081", "localhost:8082" };
        private static int nextServerIndex = 0; // Index to keep track of the next server for round-robin

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
                HandleClient(client); // Handle the client connection asynchronously
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            try
            {
                // Open streams for reading from and writing to the client
                using (NetworkStream clientStream = client.GetStream())
                using (StreamReader reader = new StreamReader(clientStream))
                using (StreamWriter writer = new StreamWriter(clientStream))
                {
                    // Read the request sent by the client
                    string request = await reader.ReadLineAsync();
                    // Log the received request from the client
                    Console.WriteLine($"Received request from {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}:\n{request}");

                    // Forward the received request to the backend server and get the response
                    string backendResponse = await ForwardRequestToBackend(request);
                    // Log the response received from the backend server
                    Console.WriteLine($"Response from backend server: {backendResponse}");

                    // Send the backend response back to the client
                    await writer.WriteLineAsync(backendResponse);
                    await writer.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during handling of the client request
                Console.WriteLine($"Error handling client request: {ex.Message}");
            }
            finally
            {
                // Close the client connection
                client.Close();
                Console.WriteLine("Client connection closed.");
            }
        }

        static async Task<string> ForwardRequestToBackend(string request)
        {
            try
            {
                // Select the next backend server using round-robin
                string backendHost = backendHosts[nextServerIndex];
                nextServerIndex = (nextServerIndex + 1) % backendHosts.Length; // Update the index for round-robin
                string[] parts = backendHost.Split(':');
                string host = parts[0];
                int port = int.Parse(parts[1]);

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
