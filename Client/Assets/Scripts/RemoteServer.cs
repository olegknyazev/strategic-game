using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    using RemoteSide = RemoteSide<StatePortion, Command>;

    public class RemoteServer : MonoBehaviour {
        Thread _thread;
        List<StatePortion> _incomingState = new List<StatePortion>();
        List<Command> _outgoingCommands = new List<Command>();
        int _running;
        int _connected;

        public bool Connected { get { return _connected == 1; } }

        public List<StatePortion> PullState() {
            List<StatePortion> messages = new List<StatePortion>();
            lock (_incomingState) {
                messages.AddRange(_incomingState);
                _incomingState.Clear();
            }
            return messages;
        }

        public void PushCommand(Command command) {
            lock (_outgoingCommands)
                _outgoingCommands.Add(command);
        }

        void Start() {
            _running = 1;
            _thread = new Thread(() => ShutdownOnError(RemoteThread));
            _thread.Start();
        }

        void OnDisable() {
            Interlocked.Exchange(ref _running, 0);
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }
        }

        void RemoteThread() {
            while (_running == 1) {
                var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
                RemoteSide remoteSide = null;
                while (_running == 1 && remoteSide == null) {
                    if ((remoteSide = TryConnect(endPoint)) == null)
                        Thread.Sleep(1000);
                };
                Interlocked.Exchange(ref _connected, 1);
                var commandsToSend = new List<Command>();
                while (_running == 1 && remoteSide.Connected) {
                    StatePortion state;
                    while ((state = remoteSide.ReadMessage()) != null)
                        lock (_incomingState)
                            _incomingState.Add(state);
                    lock (_outgoingCommands) {
                        commandsToSend.AddRange(_outgoingCommands);
                        _outgoingCommands.Clear();
                    }
                    remoteSide.WriteMessages(commandsToSend);
                    commandsToSend.Clear();
                    Thread.Sleep(10);
                }
                Interlocked.Exchange(ref _connected, 0);
            };
        }

        static void ShutdownOnError(Action a) {
            try {
                a();
            } catch (Exception ex) {
                Debug.LogErrorFormat("Exception occured in networking thread: {0}", ex);
                Application.Quit();
            }
        }

        static RemoteSide TryConnect(IPEndPoint endPoint) {
            var client = new TcpClient();
            try {
                client.Connect(endPoint);
                var remote = new RemoteSide(client, StatePortion.Deserialize);
                Debug.LogFormat("Connected to {0}", remote.RemoteEndPoint);
                client = null; // RemoteSide now owns TcpClient (see finally)
                return remote;
            } catch (SocketException ex) {
                if (ex.SocketErrorCode != SocketError.ConnectionRefused)
                    throw ex;
            } finally {
                if (client != null)
                    client.Close();
            }
            return null;
        }
    }
}
