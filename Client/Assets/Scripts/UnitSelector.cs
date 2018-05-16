using System;
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
        
        Selectable _currentSelection;
        FrameSelecting _frameSelecting;

        public Selectable CurrentSelection {
            get { return _currentSelection; }
        }

        public void SelectOne(Vector2 screenPoint) {
            SetSelection(WorldRaycaster.RaycastObject<Selectable>(screenPoint));
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
            SelectionFrame.anchoredPosition = selecting.Position;
            SelectionFrame.sizeDelta = selecting.Size;
        }
        
        void OnFrameSelectionEnd(FrameSelecting selecting) {
            Assert.AreEqual(_frameSelecting, selecting);
            SelectionFrame.gameObject.SetActive(false);
            _frameSelecting = null;
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

        class FrameSelecting : IFrameSelecting {
            readonly Vector2 _startPoint;
            Vector2 _endPoint;

            public FrameSelecting(Vector2 startPoint) {
                _startPoint = startPoint;
            }
            
            public Vector2 Position { get { return Vector2.Min(_startPoint, _endPoint); } }
            public Vector2 Size { get { return Vector2.Max(_startPoint, _endPoint) - Position; } }

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
