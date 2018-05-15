using System.Net;
using System.Net.Sockets;
using System.Threading;
using StrategicGame.Common;

namespace StrategicGame.Client {
    using RemoteServer = RemoteSide<Status, Command>;

    static class Server {
        public static RemoteServer TryConnect(IPEndPoint endPoint, ILogger logger) {
            while (true) {
                var client = new TcpClient();
                try {
                    client.Connect(endPoint);
                    var remote = new RemoteServer(client, Status.Deserialize, logger);
                    logger.Log("Connected to {0}", remote.RemoteEndPoint);
                    client = null; // RemoteSide now owns TcpClient
                    return remote;
                } catch (SocketException ex) {
                    if (ex.SocketErrorCode != SocketError.ConnectionRefused)
                        throw ex;
                } finally {
                    if (client != null)
                        client.Close();
                }
                Thread.Sleep(1000);
            };
        }
    }
}
