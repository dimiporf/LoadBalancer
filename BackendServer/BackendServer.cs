using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Define the port number for the backend server
            int port = 8080;
            // Specify the IP address to listen on (Any IP address)
            IPAddress ipAddress = IPAddress.Any;

            // Create a TCP listener on the specified IP address and port
            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start(); // Start listening for incoming TCP connections

            // Log that the backend server has started
            Console.WriteLine($"Backend server started on port {port}");

            // Accept and handle incoming client connections in a loop
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(); // Accept incoming client connection
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

                    // Prepare the response to send back to the client
                    string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nHello from the backend server";
                    // Log the response that will be sent to the client
                    Console.WriteLine($"Sending response to client:\n{response}");

                    // Send the response back to the client
                    await writer.WriteLineAsync(response);
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
            }
        }
    }
}
