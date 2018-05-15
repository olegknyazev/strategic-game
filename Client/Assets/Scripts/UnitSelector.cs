using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class UnitSelector : MonoBehaviour {
        public Camera Camera;
        
        Selectable _currentSelection;

        public Selectable CurrentSelection {
            get { return _currentSelection; }
        }

        public void SelectOne(Vector2 screenPoint) {
            var ray = Camera.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                SetSelection(hit.collider.GetComponentInParent<Selectable>());
            else
                SetSelection(null);
        }
        
        void Awake() {
            Assert.IsNotNull(Camera);
        }

        void SetSelection(Selectable selectable) {
            if (_currentSelection != selectable) {
                if (_currentSelection)
                    _currentSelection.Selected = false;
                _currentSelection = selectable;
                if (_currentSelection)
                    _currentSelection.Selected = true;
            }
        }
    }
}
