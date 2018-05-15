using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class PlayerControls : MonoBehaviour {
        const int LMB = 0;
        const int RMB = 1;

        public UnitSelector UnitSelector;

        void Awake() {
            Assert.IsNotNull(UnitSelector);    
        }

        void Update() {
            if (Input.GetMouseButtonDown(LMB))
                UnitSelector.SelectOne(Input.mousePosition);
        }
    }
}
