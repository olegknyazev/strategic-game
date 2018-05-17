using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class Unit : MonoBehaviour {
        public UnitId Id { get; private set; }
        public Movement Movement { get; private set; }

        public void Initialize(UnitId id) {
            Id = id;
            name = "Unit #" + id;
        }

        void Awake() {
            Movement = GetComponent<Movement>();
        }
    }
}
