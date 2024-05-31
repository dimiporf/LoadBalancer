using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace LoadBalancer
{
    /// <summary>
    /// Manages a pool of TCP connections for reuse.
    /// </summary>
    public class ConnectionPool
    {
        private readonly ConcurrentBag<TcpClient> _connections = new ConcurrentBag<TcpClient>();
        private readonly string _host;
        private readonly int _port;

        public ConnectionPool(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>
        /// Retrieves a connection from the pool or creates a new one if the pool is empty.
        /// </summary>
        public TcpClient GetConnection()
        {
            if (_connections.TryTake(out TcpClient connection))
            {
                Console.WriteLine($"Reusing connection to {_host}:{_port}");
                return connection;
            }

            Console.WriteLine($"Creating new connection to {_host}:{_port}");
            return new TcpClient(_host, _port);
        }

        /// <summary>
        /// Returns a connection to the pool for future reuse.
        /// </summary>
        public void ReturnConnection(TcpClient connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connections.Add(connection);
        }

        /// <summary>
        /// Closes all connections in the pool.
        /// </summary>
        public void CloseAllConnections()
        {
            foreach (var connection in _connections)
            {
                connection.Close();
            }
        }
    }
}
