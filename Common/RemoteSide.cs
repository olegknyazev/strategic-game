using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StrategicGame.Common {
    public class RemoteSide : IDisposable {
        TcpClient _client;
        NetworkStream _stream;
        ILogger _logger;
        byte[] _readBuffer = new byte[4096];
        int _readPosition;

        public RemoteSide(TcpClient client, ILogger logger) {
            _client = client;
            _stream = client.GetStream();
            _logger = logger;
        }

        public EndPoint RemoteEndPoint { get { return _client.Client.RemoteEndPoint; } }

        public Message ReadMessage() {
            if (!_stream.DataAvailable)
                return null;
            _readPosition += _stream.Read(_readBuffer, _readPosition, 4 - _readPosition);
            if (_readPosition == 4) {
                _logger.Log("Bytes read: " + string.Join(" ", _readBuffer.Select(x => x.ToString()).ToArray()));
                _readPosition = 0;
            }
            return null;
        }

        public void WriteMessage(Message msg) {
            var output = new byte[] { 25, 66, 127, 4 };
            _stream.Write(output, 0, 4);
        }
        
        public void Dispose() {
            if (_stream != null)
                _stream.Close();
            _client.Close();
        }

        protected TcpClient Client { get { return _client; } }
    }
}
