using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class Selectable : MonoBehaviour {
        bool _selected;

        public bool Selected {
            get { return _selected; }
            set {
                _selected = value;
                Debug.LogFormat("{0}.Selected = {1}", name, _selected);
            }
        }
    }
}
