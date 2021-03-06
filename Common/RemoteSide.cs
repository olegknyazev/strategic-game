﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StrategicGame.Common {
    public class RemoteSide<IncomingT, OutgoingT> : IDisposable
                where IncomingT : Message
                where OutgoingT : Message {
        const int HEADER_SIZE = 2;
        const int READ_BUFFER_SIZE = 4096;

        TcpClient _client;
        NetworkStream _stream;
        byte[] _readBuffer = new byte[READ_BUFFER_SIZE];
        MemoryStream _writeMemStream;
        BinaryReader _reader;
        BinaryWriter _writer;
        int _readPosition;
        int _expectedLength = HEADER_SIZE;
        Func<BinaryReader, IncomingT> _deserialize;

        public RemoteSide(TcpClient client, Func<BinaryReader, IncomingT> deserialize) {
            _client = client;
            _stream = client.GetStream();
            _writeMemStream = new MemoryStream();
            _reader = new BinaryReader(new MemoryStream(_readBuffer));
            _writer = new BinaryWriter(_writeMemStream);
            _deserialize = deserialize;
        }

        public EndPoint RemoteEndPoint { get { return _client.Client.RemoteEndPoint; } }

        public bool Connected { get { return _client.Client.Connected; } }

        public IncomingT ReadMessage() {
            if (!_stream.DataAvailable)
                return null;
            IncomingT result = null;
            int bytesRead = 0;
            do {
                if (DisconnectOnSocketError(() => {
                        bytesRead = _stream.Read(
                            _readBuffer,
                            _readPosition,
                            Math.Min(_expectedLength - _readPosition, READ_BUFFER_SIZE));
                    }))
                    return null;
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

        public void WriteMessage(OutgoingT msg) {
            _writer.Seek(HEADER_SIZE, SeekOrigin.Begin);
            msg.Serialize(_writer);
            var length = (ushort)_writeMemStream.Position;
            _writer.Seek(0, SeekOrigin.Begin);
            _writer.Write(length);
            DisconnectOnSocketError(() => 
                _stream.Write(_writeMemStream.GetBuffer(), 0, length));
        }

        public void WriteMessages(IEnumerable<OutgoingT> msgs) {
            foreach (var msg in msgs)
                WriteMessage(msg);
        }
        
        public void Dispose() {
            if (_stream != null)
                _stream.Close();
            _client.Close();
        }

        protected TcpClient Client { get { return _client; } }

        bool DisconnectOnSocketError(Action a) {
            try {
                a();
                return false;
            } catch (IOException ex) {
                var socketException = ex.InnerException as SocketException;
                if (socketException == null)
                    throw;
                return true;
            }
        }
    }
}
