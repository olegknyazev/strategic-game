﻿using StrategicGame.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace StrategicGame.Server {
    class Program : IDisposable {
        static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4040);

        ILogger _logger;
        List<RemoteClient> _remoteClients = new List<RemoteClient>();

        public Program(ILogger logger) {
            _logger = logger;
        }

        public void Execute() {
            var acceptor = new ClientAcceptor(endPoint);
            while (true) {
                IntroduceNewClients(acceptor);
            }
        }

        public void Dispose() {
            foreach (var client in _remoteClients)
                client.Dispose();
        }
        
        void IntroduceNewClients(ClientAcceptor acceptor) {
            TcpClient client;
            while ((client = acceptor.PullClient()) != null) {
                var remoteClient = new RemoteClient(client);
                _logger.Log("Client connected: {0}", remoteClient.RemoteEndPoint);
                _remoteClients.Add(remoteClient);
            }
        }

        static void Main(string[] args) {
            using (var program = new Program(ConsoleLogger.Instance))
                program.Execute();
        }
    }
}
