using System;
using System.Net;
using System.Threading;
using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class Client : MonoBehaviour {
        Thread _thread;
        bool _running;

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

        private void RemoteThread() {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
            var remoteServer = RemoteServer.TryConnect(endPoint, UnityConsoleLogger.Instance);
            while (_running) {
                Message msg;
                while ((msg = remoteServer.ReadMessage()) != null)
                    Debug.LogFormat("Message received: {0}", msg);
                Thread.Sleep(10);
            }
        }
    }
}
