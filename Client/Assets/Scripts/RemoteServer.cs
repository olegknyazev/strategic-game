using System.Net;
using System.Net.Sockets;
using System.Threading;
using StrategicGame.Common;

namespace StrategicGame.Client {
    static class RemoteServer {
        public static RemoteSide TryConnect(IPEndPoint endPoint, ILogger logger) {
            while (true) {
                var client = new TcpClient();
                try {
                    client.Connect(endPoint);
                    var remote = new RemoteSide(client, logger);
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
