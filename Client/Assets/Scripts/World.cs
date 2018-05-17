using System.Collections.Generic;
using UnityEngine;
using StrategicGame.Common;

namespace StrategicGame.Client {
    public class World : MonoBehaviour {
        public GameObject CellPrefab;
        public Unit UnitPrefab;

        Transform _cellsRoot;
        Transform _unitsRoot;
        WorldParameters _params;
        Dictionary<UnitId, Unit> _units = new Dictionary<UnitId, Unit>();

        public Vector3 Center {
            get { return new Vector3(_params.Width / 2f, 0, _params.Height / 2f); }
        }

        public void Initialize(WorldParameters worldParams) {
            Debug.Assert(_params == null);
            _params = worldParams;
            _cellsRoot = CreateGroup("Cells");
            _unitsRoot = CreateGroup("Units");
            for (int x = 0; x < worldParams.Width; ++x)
                for (int z = 0; z < worldParams.Height; ++z)
                    GameObject.Instantiate(
                        CellPrefab,
                        new Vector3(x, 0, z),
                        Quaternion.identity,
                        _cellsRoot);
        }

        public void UpdateUnitPosition(UnitPosition cmd) {
            bool created;
            var unit = GetOrInstantiate(cmd.Id, out created);
            if (unit.Movement) {
                unit.Movement.SetPosition(new Vector3(cmd.X, 0, cmd.Y), interpolate: !created);
                unit.Movement.Moving = cmd.Moving;
            }
        }

        Unit GetOrInstantiate(UnitId id, out bool created) {
            created = false;
            Unit unit;
            if (!_units.TryGetValue(id, out unit)) {
                unit = GameObject.Instantiate(UnitPrefab, _unitsRoot);
                unit.Initialize(id);
                _units.Add(id, unit);
                created = true;
            }
            return unit;
        }
        
        Transform CreateGroup(string name) {
            var transform = new GameObject(name).transform;
            transform.SetParent(this.transform, false);
            return transform;
        }
    }
}
