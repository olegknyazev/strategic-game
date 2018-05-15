using UnityEngine;
using UnityEngine.Assertions;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class Client : MonoBehaviour {
        public RemoteServer RemoteServer;
        public Camera Camera;

        [Header("Prefabs")]
        public World WorldPrefab;
        
        World _world;

        void Awake() {
            Assert.IsNotNull(RemoteServer);
            Assert.IsNotNull(Camera);
            Assert.IsNotNull(WorldPrefab);
        }

        void Update() {
            foreach (var msg in RemoteServer.PullMessages())
                ProcessMessage(msg);
        }

        void ProcessMessage(Status msg) {
            Debug.Log("PROCESS: " + msg);
            if (msg is WorldParameters)
                RecreateWorld((WorldParameters)msg);
            else if (msg is UnitPosition)
                UpdateUnitPosition((UnitPosition)msg);
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

        void UpdateUnitPosition(UnitPosition unitParams) {
            _world.UpdateUnitPosition(unitParams);
        }
    }
}
