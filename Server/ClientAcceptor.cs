using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StrategicGame.Server {
    class ClientAcceptor {
        Thread _thread;
        Queue<TcpClient> _pendingClients = new Queue<TcpClient>();

        public ClientAcceptor(IPEndPoint endPoint) {
            _thread = new Thread(() => AcceptClients(endPoint));
            _thread.Start();
        }

        public TcpClient PullClient() {
            lock (_pendingClients) {
                return _pendingClients.Count > 0
                    ? _pendingClients.Dequeue()
                    : null;
            }
        }

        void AcceptClients(IPEndPoint endPoint) {
            var listener = new TcpListener(endPoint);
            listener.Start();
            while (true) {
                var client = listener.AcceptTcpClient();
                lock (_pendingClients)
                    _pendingClients.Enqueue(client);
            }
        }
    }
}
