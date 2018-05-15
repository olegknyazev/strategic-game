using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    using RemoteSide = RemoteSide<Status, Command>;

    public class RemoteServer : MonoBehaviour {
        Thread _thread;
        bool _running;
        List<Status> _incomingMessages = new List<Status>();

        public List<Status> PullMessages() {
            List<Status> messages = new List<Status>();
            lock (_incomingMessages) {
                messages.AddRange(_incomingMessages);
                _incomingMessages.Clear();
            }
            return messages;
        }

        void Start() {
            _running = true;
            _thread = new Thread(RemoteThread);
            _thread.Start();
        }

        void OnDisable() {
            _running = false;
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }
        }

        void RemoteThread() {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
            var remoteSide = TryConnect(endPoint, UnityConsoleLogger.Instance);
            while (_running) {
                Status msg;
                while ((msg = remoteSide.ReadMessage()) != null)
                    lock (_incomingMessages)
                        _incomingMessages.Add(msg);
                Thread.Sleep(10);
            }
        }

        static RemoteSide TryConnect(IPEndPoint endPoint, Common.ILogger logger) {
            while (true) {
                var client = new TcpClient();
                try {
                    client.Connect(endPoint);
                    var remote = new RemoteSide(client, Status.Deserialize, logger);
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
