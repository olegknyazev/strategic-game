using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class Movement : MonoBehaviour {
        Vector3 _startPosition;
        Vector3 _destination;
        float _destinationSetTime;

        public Vector3 Position {
            get { return transform.position; }
        }

        public void SetPosition(Vector3 newPosition, bool interpolate = true) {
            transform.position = _startPosition = interpolate ? Position : newPosition;
            _destination = newPosition;
            _destinationSetTime = Time.time;
        }

        void Awake() {
            _startPosition = _destination = Position;
        }

        void Update() {
            if (Position != _destination) {
                var logicStepLength = 1f / Consts.STEPS_PER_SECOND;
                var progress = (Time.time - _destinationSetTime) / logicStepLength;
                transform.position = Vector3.Lerp(_startPosition, _destination, progress);
            }
        }
    }
}
