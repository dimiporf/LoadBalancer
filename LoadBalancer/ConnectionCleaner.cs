using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LoadBalancer
{
    /// <summary>
    /// Class responsible for cleaning up idle connections.
    /// </summary>
    class ConnectionCleaner
    {
        private static readonly TimeSpan ConnectionIdleTimeout = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Starts the connection cleanup task.
        /// </summary>
        public static async Task StartConnectionCleanup()
        {
            while (true)
            {
                foreach (var kvp in BackendCommunicator.ConnectionPools)
                {
                    var backendServer = kvp.Key;
                    var connectionPool = kvp.Value;

                    var connectionsToClean = new ConcurrentBag<TcpClient>();
                    foreach (var connection in connectionPool)
                    {
                        if (!connection.Connected || IsConnectionIdle(connection))
                        {
                            connectionsToClean.Add(connection);
                        }
                    }

                    while (connectionsToClean.TryTake(out var connection))
                    {
                        connection.Close();
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private static bool IsConnectionIdle(TcpClient connection)
        {
            // Implement logic to check if the connection is idle.
            // This could be based on a timestamp of the last use or other criteria.
            return false;
        }
    }
}
