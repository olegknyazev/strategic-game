using System.Net;
using System.Net.Sockets;
using StrategicGame.Common;

namespace StrategicGame.Client {
    class RemoteServer : RemoteBase {
        public RemoteServer(IPEndPoint endPoint) : base(new TcpClient()) {
            Client.Connect(endPoint);
        }
    }
}
