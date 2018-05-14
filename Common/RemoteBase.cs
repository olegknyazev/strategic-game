using System;
using System.Net;
using System.Net.Sockets;

namespace StrategicGame.Common {
    public class RemoteBase : IDisposable {
        TcpClient _client;

        public RemoteBase(TcpClient client) {
            _client = client;
        }

        public EndPoint RemoteEndPoint { get { return _client.Client.RemoteEndPoint; } }

        public Message PullMessage() { return null; }

        public void PushMessage(Message msg) { }
        
        public void Dispose() {
            _client.Close();
        }

        protected TcpClient Client { get { return _client; } }
    }
}
