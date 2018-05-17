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
        ConcurrentQueue<StatePortion> _incomingState = new ConcurrentQueue<StatePortion>();
        ConcurrentQueue<Command> _outgoingCommands = new ConcurrentQueue<Command>();
        int _running;
        int _connected;

        public bool Connected { get { return _connected == 1; } }

        public StatePortion PullState() {
            return _incomingState.Dequeue();
        }

        public void PushCommand(Command command) {
            _outgoingCommands.Enqueue(command);
        }

        void Start() {
            _running = 1;
            _thread = new Thread(() => ShutdownOnError(NetworkThread));
            _thread.Start();
        }

        void OnDisable() {
            Interlocked.Exchange(ref _running, 0);
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }
        }

        void NetworkThread() {
            while (_running == 1) {
                var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
                RemoteSide remoteSide = null;
                while (_running == 1 && remoteSide == null) {
                    if ((remoteSide = TryConnect(endPoint)) == null)
                        Thread.Sleep(1000);
                }
                Interlocked.Exchange(ref _connected, 1);
                while (_running == 1 && remoteSide.Connected) {
                    StatePortion state;
                    while ((state = remoteSide.ReadMessage()) != null)
                        _incomingState.Enqueue(state);
                    Command cmd;
                    while ((cmd = _outgoingCommands.Dequeue()) != null)
                        remoteSide.WriteMessage(cmd);
                    Thread.Sleep(1);
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
