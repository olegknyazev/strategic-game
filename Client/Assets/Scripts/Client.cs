using StrategicGame.Common;
using System;
using System.Net;
using System.Threading;
using UnityEngine;

namespace StrategicGame.Client {
    public class Client : MonoBehaviour {
        Thread _thread;
        bool _running;

        void Start() {
            _thread = new Thread(RemoteThread);
            _thread.Start();
            _running = true;
        }

        void OnDisable() {
            _running = false;
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }
        }

        private void RemoteThread() {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
            var remoteServer = RemoteServer.TryConnect(endPoint, UnityConsoleLogger.Instance);
            remoteServer.WriteMessage(null); // Hello!
            while (_running) {
                Message msg;
                do {
                    msg = remoteServer.ReadMessage();
                    if (msg != null)
                        Debug.LogFormat("Message received: {0}", msg);
                } while (msg != null);
            }
        }
    }
}
