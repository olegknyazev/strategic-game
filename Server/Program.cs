﻿using StrategicGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _world = World.RandomWorld(Consts.STEPS_PER_SECOND);
        }

        public void Execute() {
            var acceptor = new ClientAcceptor(endPoint);
            var clientCommands = new List<Command>();
            while (true) {
                IntroduceNewClients(acceptor);
                foreach (var client in _remoteClients) {
                    Command cmd;
                    while ((cmd = client.ReadMessage()) != null) {
                        clientCommands.Add(cmd);
                        _logger.Log("{0} says {1}", client.RemoteEndPoint, cmd);
                    }
                }
                var statusMessages = _world.Simulate(clientCommands);
                clientCommands.Clear();
                foreach (var client in _remoteClients)
                    client.WriteMessages(statusMessages);
                Thread.Sleep(1000 / Consts.STEPS_PER_SECOND);
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
