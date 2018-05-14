using System;
using System.Net.Sockets;
using StrategicGame.Common;

namespace StrategicGame.Server {
    class RemoteClient : RemoteBase {
        public RemoteClient(TcpClient client) : base(client) {
        }
    }
}
