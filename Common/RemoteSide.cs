using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StrategicGame.Common {
    public class RemoteSide : IDisposable {
        const int HEADER_SIZE = 2;

        TcpClient _client;
        NetworkStream _stream;
        ILogger _logger;
        byte[] _readBuffer = new byte[4096];
        byte[] _writeBuffer = new byte[4096];
        BinaryReader _reader;
        BinaryWriter _writer;
        int _readPosition;
        int _expectedLength = HEADER_SIZE;
        Func<BinaryReader, Message> _deserialize;

        public RemoteSide(TcpClient client, Func<BinaryReader, Message> deserialize, ILogger logger) {
            _client = client;
            _stream = client.GetStream();
            _logger = logger;
            _reader = new BinaryReader(new MemoryStream(_readBuffer));
            _writer = new BinaryWriter(new MemoryStream(_writeBuffer));
            _deserialize = deserialize;
        }

        public EndPoint RemoteEndPoint { get { return _client.Client.RemoteEndPoint; } }

        public Message ReadMessage() {
            if (!_stream.DataAvailable)
                return null;
            Message result = null;
            int bytesRead = 0;
            do {
                bytesRead = _stream.Read(_readBuffer, _readPosition, _expectedLength - _readPosition);
                _readPosition += bytesRead;
                if (_readPosition >= HEADER_SIZE) {
                    if (_expectedLength == HEADER_SIZE) {
                        _reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        _expectedLength = _reader.ReadUInt16();
                    }
                    if (_readPosition == _expectedLength) {
                        _reader.BaseStream.Seek(HEADER_SIZE, SeekOrigin.Begin);
                        result = _deserialize(_reader);
                        _expectedLength = HEADER_SIZE;
                        _readPosition = 0;
                    }
                }
            } while (bytesRead > 0 && result == null);
            return result;
        }

        public void WriteMessage(Message msg) {
            _writer.Seek(HEADER_SIZE, SeekOrigin.Begin);
            msg.Serialize(_writer);
            var length = (ushort)_writer.BaseStream.Position;
            _writer.Seek(0, SeekOrigin.Begin);
            _writer.Write(length);
            _stream.Write(_writeBuffer, 0, HEADER_SIZE + length);
        }

        public void WriteMessages(IEnumerable<Message> msgs) {
            foreach (var msg in msgs)
                WriteMessage(msg);
        }
        
        public void Dispose() {
            if (_stream != null)
                _stream.Close();
            _client.Close();
        }

        protected TcpClient Client { get { return _client; } }
    }
}
