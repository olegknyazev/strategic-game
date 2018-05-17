using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class Movement : MonoBehaviour {
        Vector3 _startPosition;
        Vector3 _destination;
        float _destinationSetTime;
        Animator _animator;
        bool _moving;

        public Vector3 Position {
            get { return transform.localPosition; }
        }

        public bool Moving {
            get { return _moving; }
            set {
                if (_moving != value) {
                    _moving = value;
                    _animator.SetBool("Moving", value);
                }
            }
        }

        public void SetPosition(Vector3 newPosition, bool interpolate = true) {
            transform.localPosition = _startPosition = interpolate ? Position : newPosition;
            transform.localRotation = interpolate ? FromToRotation(Position, newPosition) : Quaternion.identity;
            _destination = newPosition;
            _destinationSetTime = Time.time;
        }

        void Awake() {
            _animator = GetComponent<Animator>();
            _startPosition = _destination = Position;
        }

        void Update() {
            if (Position != _destination) {
                var logicStepLength = 1f / Consts.STEPS_PER_SECOND;
                var progress = (Time.time - _destinationSetTime) / logicStepLength;
                transform.localPosition = Vector3.Lerp(_startPosition, _destination, progress);
                transform.localRotation = FromToRotation(_startPosition, _destination);
            }
        }

        static Quaternion FromToRotation(Vector3 from, Vector3 to) {
            return Quaternion.LookRotation(to - from, Vector3.up);
        }
    }
}
