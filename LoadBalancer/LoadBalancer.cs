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
            int port = 80;
            IPAddress ipAddress = IPAddress.Any;

            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();

            Console.WriteLine($"Load balancer started on port {port}");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                HandleClient(client);
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            try
            {
                using (NetworkStream clientStream = client.GetStream())
                using (StreamReader reader = new StreamReader(clientStream))
                using (StreamWriter writer = new StreamWriter(clientStream))
                {
                    string request = await reader.ReadLineAsync();
                    Console.WriteLine($"Received request from {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}:\n{request}");

                    string backendResponse = await ForwardRequestToBackend(request);
                    Console.WriteLine($"Response from backend server: {backendResponse}");

                    await writer.WriteLineAsync(backendResponse);
                    await writer.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client request: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        static async Task<string> ForwardRequestToBackend(string request)
        {
            try
            {
                string backendHost = "localhost";
                int backendPort = 8080;

                using (TcpClient backendClient = new TcpClient())
                {
                    Console.WriteLine($"Connecting to backend server {backendHost}:{backendPort}");
                    await backendClient.ConnectAsync(backendHost, backendPort);

                    using (NetworkStream stream = backendClient.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Console.WriteLine("Sending request to backend server...");
                        await writer.WriteLineAsync(request);
                        await writer.FlushAsync();

                        Console.WriteLine("Waiting for response from backend server...");
                        string response = await reader.ReadToEndAsync();
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error communicating with backend server: {ex.Message}");
                return "HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/plain\r\n\r\nError communicating with backend server";
            }
        }
    }
}
