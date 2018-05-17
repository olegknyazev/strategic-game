﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using StrategicGame.Common;

namespace StrategicGame.Server {
    using RemoteClient = RemoteSide<Command, Status>;

    class Program : IDisposable {
        static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4040);

        World _world;
        List<RemoteClient> _remoteClients = new List<RemoteClient>();

        public Program() {
            _world = World.RandomWorld(Consts.STEPS_PER_SECOND);
        }

        public void Execute() {
            var acceptor = new ClientAcceptor(endPoint);
            var clientCommands = new List<Command>();
            while (true) {
                IntroduceNewClients(acceptor);
                RemoveDisconnectedClients();
                FetchClientCommands(clientCommands);
                var statusMessages = _world.Simulate(clientCommands);
                clientCommands.Clear();
                SendStatusToClients(statusMessages);
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
                var remoteClient = new RemoteClient(client, Command.Deserialize);
                Log("Client connected: {0}", remoteClient.RemoteEndPoint);
                remoteClient.WriteMessages(_world.Status);
                _remoteClients.Add(remoteClient);
            }
        }
        
        void RemoveDisconnectedClients() {
            for (int i = _remoteClients.Count - 1; i >= 0; --i) {
                var client = _remoteClients[i];
                if (!client.Connected) {
                    Log("Client disconnected: {0}", client.RemoteEndPoint);
                    client.Dispose();
                    _remoteClients.RemoveAt(i);
                }
            }
        }
        
        void FetchClientCommands(List<Command> clientCommands) {
            foreach (var client in _remoteClients) {
                Command cmd;
                while ((cmd = client.ReadMessage()) != null) {
                    clientCommands.Add(cmd);
                    Log("{0} says: {1}", client.RemoteEndPoint, cmd);
                }
            }
        }
        
        void SendStatusToClients(List<Status> statusMessages) {
            foreach (var client in _remoteClients)
                client.WriteMessages(statusMessages);
        }

        static void Main(string[] args) {
            using (var program = new Program())
                program.Execute();
        }

        static void Log(string message, params object[] args) {
            Console.WriteLine(message, args);
        }
    }
}
