using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class PlayerControls : MonoBehaviour {
        const int LMB = 0;
        const int RMB = 1;

        public UnitSelector UnitSelector;
        public WorldRaycaster WorldRaycaster;

        Vector2 _mouseDownPosition;
        bool _mousePressed;
        UnitSelector.IFrameSelecting _frameSelecting;

        public event Action<Unit, Vector3> OnMoveOrder;

        void Awake() {
            Assert.IsNotNull(UnitSelector);    
            Assert.IsNotNull(WorldRaycaster);  
        }

        void Update() {
            var mousePosition = (Vector2)Input.mousePosition;
            if (Input.GetMouseButtonDown(LMB)) {
                _mouseDownPosition = mousePosition;
                _mousePressed = true;
            } else if (Input.GetMouseButtonUp(LMB)) {
                if (!_mousePressed)
                    UnitSelector.SelectOne(mousePosition);
                if (_frameSelecting != null) {
                    _frameSelecting.End();
                    _frameSelecting = null;
                }
                _mousePressed = false;
            } else if (Input.GetMouseButtonDown(RMB)) {
                var selection = UnitSelector.CurrentSelection;
                var unit = selection ? selection.GetComponent<Unit>() : null;
                if (unit) {
                    var groundPoint =  WorldRaycaster.RaycastGround(mousePosition);
                    OnMoveOrder.InvokeSafe(unit, groundPoint);
                    UnitSelector.Deselect();
                }
            } else if (_mousePressed) {
                if (_frameSelecting != null)
                    _frameSelecting.Update(mousePosition);
                else if ((mousePosition - _mouseDownPosition).magnitude > 5)
                    _frameSelecting = UnitSelector.BeginFrameSelection(mousePosition);
            }
        }
    }
}
