using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class Movement : MonoBehaviour {
        Animator _animator;
        Vector3 _startPosition;
        Vector3 _destination;
        float _destinationSetTime;
        bool _moving;

        public Vector3 Position {
            get { return transform.localPosition; }
        }

        public bool Moving {
            get { return _moving; }
            set {
                if (_moving != value) {
                    _moving = value;
                    if (_animator)
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
            var delta = to - from;
            return delta.sqrMagnitude > 0.001f 
                ? Quaternion.LookRotation(delta, Vector3.up)
                : Quaternion.identity;
        }
    }
}
