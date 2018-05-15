using StrategicGame.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StrategicGame.Server {
    using RemoteClient = RemoteSide<Command, Status>;

    class Program : IDisposable {
        static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4040);

        ILogger _logger;
        World _world;
        List<RemoteClient> _remoteClients = new List<RemoteClient>();

        public Program(ILogger logger) {
            _logger = logger;
            _world = World.RandomWorld();
        }

        public void Execute() {
            var acceptor = new ClientAcceptor(endPoint);
            while (true) {
                IntroduceNewClients(acceptor);
                foreach (var client in _remoteClients) {
                    Message msg;
                    while ((msg = client.ReadMessage()) != null)
                        _logger.Log("{0} says {1}", client.RemoteEndPoint, msg);
                }
                Thread.Sleep(10);
            }
        }

        public void Dispose() {
            foreach (var client in _remoteClients)
                client.Dispose();
        }
        
        void IntroduceNewClients(ClientAcceptor acceptor) {
            TcpClient client;
            while ((client = acceptor.PullClient()) != null) {
                var remoteClient = new RemoteClient(client, Command.Deserialize, _logger);
                _logger.Log("Client connected: {0}", remoteClient.RemoteEndPoint);
                remoteClient.WriteMessages(_world.Status);
                _remoteClients.Add(remoteClient);
            }
        }

        static void Main(string[] args) {
            using (var program = new Program(ConsoleLogger.Instance))
                program.Execute();
        }
    }
}
