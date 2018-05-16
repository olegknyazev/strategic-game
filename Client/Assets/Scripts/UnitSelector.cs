using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace StrategicGame.Client {
    public class UnitSelector : MonoBehaviour {
        public WorldRaycaster WorldRaycaster;
        public RectTransform SelectionFrame;

        public interface IFrameSelecting {
            void Update(Vector2 screenPoint);
            void End();
        }
        
        List<Selectable> _currentSelection = new List<Selectable>();
        FrameSelecting _frameSelecting;

        public IEnumerable<Selectable> CurrentSelection {
            get { return _currentSelection; }
        }

        public void SelectOne(Vector2 screenPoint) {
            var selectable = WorldRaycaster.RaycastObject<Selectable>(screenPoint);
            SetSelection(selectable != null ? new[] { selectable } : null);
        }

        public IFrameSelecting BeginFrameSelection(Vector2 startPoint) {
            if (_frameSelecting != null) {
                _frameSelecting.OnUpdate -= OnFrameSelectionUpdate;
                _frameSelecting.OnEnd -= OnFrameSelectionEnd;
                _frameSelecting.End();
            }
            _frameSelecting = new FrameSelecting(startPoint);
            _frameSelecting.OnUpdate += OnFrameSelectionUpdate;
            _frameSelecting.OnEnd += OnFrameSelectionEnd;
            return _frameSelecting;
        }

        public void Deselect() {
            SetSelection(null);
        }
        
        void Awake() {
            Assert.IsNotNull(WorldRaycaster);
            Assert.IsNotNull(SelectionFrame);
        }
        
        void OnFrameSelectionUpdate(FrameSelecting selecting) {
            Assert.AreEqual(_frameSelecting, selecting);
            SelectionFrame.gameObject.SetActive(true);
            SelectionFrame.anchoredPosition = selecting.Min;
            SelectionFrame.sizeDelta = selecting.Max - selecting.Min;
        }
        
        void OnFrameSelectionEnd(FrameSelecting selecting) {
            Assert.AreEqual(_frameSelecting, selecting);
            SetSelection(FindSelectables(selecting));
            SelectionFrame.gameObject.SetActive(false);
            _frameSelecting = null;
        }

        IEnumerable<Selectable> FindSelectables(FrameSelecting selecting) {
            var p1 = WorldRaycaster.RaycastGround(selecting.Min);
            var p2 = WorldRaycaster.RaycastGround(selecting.Max);
            var size = p2 - p1;
            size.y = 10;
            var b = new Bounds((p1 + p2) / 2, size);
            return WorldRaycaster.OverlapBox<Selectable>(b);
        }

        void SetSelection(IEnumerable<Selectable> selectables) {
            foreach (var s in _currentSelection)
                s.Selected = false;
            _currentSelection.Clear();
            if (selectables != null)
                _currentSelection.AddRange(selectables);
            foreach (var s in _currentSelection)
                s.Selected = true;
        }

        class FrameSelecting : IFrameSelecting {
            readonly Vector2 _startPoint;
            Vector2 _endPoint;

            public FrameSelecting(Vector2 startPoint) {
                _startPoint = startPoint;
            }
            
            public Vector2 Min { get { return Vector2.Min(_startPoint, _endPoint); } }
            public Vector2 Max { get { return Vector2.Max(_startPoint, _endPoint); } }

            public event Action<FrameSelecting> OnUpdate;
            public event Action<FrameSelecting> OnEnd;

            public void Update(Vector2 screenPoint) {
                _endPoint = screenPoint;
                OnUpdate.InvokeSafe(this);
            }

            public void End() {
                OnEnd.InvokeSafe(this);
            }
        }
    }
}
