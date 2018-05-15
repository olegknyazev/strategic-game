using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class WorldRaycaster : MonoBehaviour {
        public Camera Camera;
        
        public T RaycastObject<T>(Vector2 screenPoint) where T : Component {
            var ray = Camera.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                return hit.collider.GetComponentInParent<T>();
            return null;
        }

        public Vector3 RaycastGround(Vector2 screenPoint) {
            var ray = Camera.ScreenPointToRay(screenPoint);
            var groundPlane = new Plane(Vector3.up, 0);
            float distance;
            if (groundPlane.Raycast(ray, out distance))
                return ray.GetPoint(distance);
            Debug.LogErrorFormat("Invalid ground raycast: {0}", ray);
            return Vector3.zero;
        }
        
        void Awake() {
            Assert.IsNotNull(Camera);
        }
    }
}
