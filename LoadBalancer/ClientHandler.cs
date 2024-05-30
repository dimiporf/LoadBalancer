using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LoadBalancer
{
    /// <summary>
    /// Class responsible for handling client connections.
    /// </summary>
    class ClientHandler
    {
        /// <summary>
        /// Handles an incoming client connection.
        /// </summary>
        /// <param name="client">The TCP client representing the connected client.</param>
        public static async Task HandleClient(TcpClient client)
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
                    string backendResponse = await BackendCommunicator.ForwardRequestToBackend(request);
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
    }
}
