using UnityEngine;

namespace StrategicGame.Client {
    public class Selectable : MonoBehaviour {
        public Material NormalMaterial;
        public Material SelectedMaterial;

        bool _selected;
        MeshRenderer _materialTarget;

        void Awake() {
            _materialTarget = GetComponentInChildren<MeshRenderer>();
        }

        public bool Selected {
            get { return _selected; }
            set {
                if (_selected != value) {
                    _selected = value;
                    UpdateMaterial();
                }
            }
        }

        void UpdateMaterial() {
            if (_materialTarget)
                _materialTarget.sharedMaterial = _selected ? SelectedMaterial : NormalMaterial;
        }
    }
}
