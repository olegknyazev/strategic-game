using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class PlayerControls : MonoBehaviour {
        const int LMB = 0;
        const int RMB = 1;

        public UnitSelector UnitSelector;
        public WorldRaycaster WorldRaycaster;

        public event Action<Unit, Vector3> OnMoveOrder;

        void Awake() {
            Assert.IsNotNull(UnitSelector);    
            Assert.IsNotNull(WorldRaycaster);  
        }

        void Update() {
            if (Input.GetMouseButtonDown(LMB))
                UnitSelector.SelectOne(Input.mousePosition);
            else if (Input.GetMouseButtonDown(RMB)) {
                var unit = UnitSelector.CurrentSelection.GetComponent<Unit>();
                if (unit) {
                    var groundPoint =  WorldRaycaster.RaycastGround(Input.mousePosition);
                    OnMoveOrder.InvokeSafe(unit, groundPoint);
                }
            }
        }
    }
}
