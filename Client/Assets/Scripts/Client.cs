using UnityEngine;
using UnityEngine.Assertions;
using StrategicGame.Common;
using System.Collections.Generic;
using System.Linq;

namespace StrategicGame.Client {
    public class Client : MonoBehaviour {
        public RemoteServer RemoteServer;
        public PlayerControls PlayerControls;
        public Camera Camera;
        public GameObject ConnectingOverlay;

        [Header("Prefabs")]
        public World WorldPrefab;
        
        World _world;

        void Awake() {
            Assert.IsNotNull(RemoteServer);
            Assert.IsNotNull(PlayerControls);
            Assert.IsNotNull(Camera);
            Assert.IsNotNull(WorldPrefab);
        }

        void OnEnable() {
            PlayerControls.OnMoveOrder += OnMoveOrder;
        }

        void OnDisable() {
            PlayerControls.OnMoveOrder -= OnMoveOrder;
        }

        void Update() {
            if (RemoteServer.Connected) {
                StatePortion state;
                while ((state = RemoteServer.PullState()) != null)
                    ProcessState(state);
                if (ConnectingOverlay)
                    ConnectingOverlay.SetActive(false);
            } else {
                DestroyWorld();
                if (ConnectingOverlay)
                    ConnectingOverlay.SetActive(true);
            }
        }
        
        void OnMoveOrder(IEnumerable<Unit> units, Vector3 destination) {
            var destinationCell = new Int2(
                Mathf.RoundToInt(destination.x),
                Mathf.RoundToInt(destination.z));
            RemoteServer.PushCommand(new SendUnits(units.Select(u => u.Id), destinationCell));
        }

        void ProcessState(StatePortion state) {
            if (state is WorldParameters)
                RecreateWorld((WorldParameters)state);
            else if (state is UnitPosition)
                UpdateUnitPosition((UnitPosition)state);
        }

        void RecreateWorld(WorldParameters worldParams) {
            DestroyWorld();
            _world = GameObject.Instantiate(WorldPrefab, transform);
            _world.name = "World";
            _world.Initialize(worldParams);
            Camera.transform.position = _world.Center + Vector3.up * 10;
            Camera.transform.LookAt(_world.Center);
        }

        void DestroyWorld() {
            if (_world) {
                DestroyImmediate(_world.gameObject);
                _world = null;
            }
        }

        void UpdateUnitPosition(UnitPosition state) {
            _world.UpdateUnitPosition(state);
        }
    }
}
