using UnityEngine;

namespace StrategicGame.Client {
    public class Unit : MonoBehaviour {
        public uint Id { get; private set; }

        public void Initialize(uint id) {
            Id = id;
            name = "Unit #" + id;
        }
    }
}
