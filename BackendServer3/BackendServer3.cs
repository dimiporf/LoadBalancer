using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer3
{
    class BackendServer3
    {
        static async Task Main(string[] args)
        {
            int port = 8082; // Port for the third backend server
            IPAddress ipAddress = IPAddress.Any;

            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();

            Console.WriteLine($"Backend server 3 started on port {port}");

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

                    string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nHello from backend server 3";
                    Console.WriteLine($"Sending response to client:\n{response}");

                    await writer.WriteLineAsync(response);
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
    }
}
