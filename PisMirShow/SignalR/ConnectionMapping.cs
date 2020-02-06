using System.Collections.Generic;
using System.Linq;

namespace PisMirShow.SignalR
{
    public class ConnectionMapping<T>
    {
        private static readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public static int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

		public IReadOnlyList<string> GetConnectionsStrings(T key)
		{
			HashSet<string> connections;
			if (_connections.TryGetValue(key, out connections))
			{
				return connections.ToList();
			}

			return (IReadOnlyList<string>)Enumerable.Empty<string>();
		}

        public bool GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return true;
            }

            return false;
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}
