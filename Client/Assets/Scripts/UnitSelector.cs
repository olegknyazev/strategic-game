using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class UnitSelector : MonoBehaviour {
        public WorldRaycaster WorldRaycaster;
        
        Selectable _currentSelection;

        public Selectable CurrentSelection {
            get { return _currentSelection; }
        }

        public void SelectOne(Vector2 screenPoint) {
            SetSelection(WorldRaycaster.RaycastObject<Selectable>(screenPoint));
        }

        public void Deselect() {
            SetSelection(null);
        }
        
        void Awake() {
            Assert.IsNotNull(WorldRaycaster);
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
