using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class World : MonoBehaviour {
        public GameObject CellPrefab;

        WorldParameters _params;

        public Vector3 Center {
            get { return new Vector3(_params.Width / 2f, 0, _params.Height / 2f); }
        }

        public void Initialize(WorldParameters worldParams) {
            Debug.Assert(_params == null);
            _params = worldParams;
            for (int x = 0; x < worldParams.Width; ++x)
                for (int z = 0; z < worldParams.Height; ++z)
                    GameObject.Instantiate(CellPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
        }
    }
}
