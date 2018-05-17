using System.Net;
using System.Net.Sockets;
using System.Threading;
using StrategicGame.Common;

namespace StrategicGame.Server {
    class ClientAcceptor {
        Thread _thread;
        ConcurrentQueue<TcpClient> _pendingClients = new ConcurrentQueue<TcpClient>();

        public ClientAcceptor(IPEndPoint endPoint) {
            _thread = new Thread(() => AcceptClients(endPoint));
            _thread.Start();
        }

        public TcpClient PullClient() {
            return _pendingClients.Dequeue();
        }

        void AcceptClients(IPEndPoint endPoint) {
            var listener = new TcpListener(endPoint);
            listener.Start();
            while (true)
                _pendingClients.Enqueue(listener.AcceptTcpClient());
        }
    }
}
