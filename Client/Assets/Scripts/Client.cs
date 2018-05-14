using System;
using System.Net;
using System.Threading;
using UnityEngine;
using StrategicGame.Common;
using System.Collections.Generic;

namespace StrategicGame.Client {
    public class Client : MonoBehaviour {
        public World WorldPrefab;
        public Camera Camera;

        Thread _thread;
        bool _running;
        List<Message> _incomingMessages = new List<Message>();
        World _world;

        void Start() {
            _running = true;
            _thread = new Thread(RemoteThread);
            _thread.Start();
        }

        void Update() {
            List<Message> messagesToProcess = new List<Message>();
            lock (_incomingMessages) {
                messagesToProcess.AddRange(_incomingMessages);
                _incomingMessages.Clear();
            }
            foreach (var msg in messagesToProcess)
                if (msg is WorldParameters)
                    RecreateWorld((WorldParameters)msg);
        }

        void OnDisable() {
            _running = false;
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }
        }

        void RecreateWorld(WorldParameters worldParams) {
            if (_world) {
                DestroyImmediate(_world.gameObject);
                _world = null;
            }
            _world = GameObject.Instantiate(WorldPrefab, transform);
            _world.Initialize(worldParams);
            Camera.transform.position = _world.Center + Vector3.up * 10;
            Camera.transform.LookAt(_world.Center);
        }

        void RemoteThread() {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
            var remoteServer = RemoteServer.TryConnect(endPoint, UnityConsoleLogger.Instance);
            while (_running) {
                Message msg;
                while ((msg = remoteServer.ReadMessage()) != null)
                    lock (_incomingMessages)
                        _incomingMessages.Add(msg);
                Thread.Sleep(10);
            }
        }
    }
}
