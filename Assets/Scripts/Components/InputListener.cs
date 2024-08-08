using Events;
using Extensions.System;
using Extensions.Unity;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using Zenject;

namespace Components
{
    public class InputListener : EventListenerMono
    {
        private const float ZoomDeltaThreshold = 0.01f;
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private Camera Camera{get;set;}
        [Inject] private GridEvents GridEvents{get;set;}
        private RoutineHelper _inputRoutine;
        private float _lastDist;
        private int _lastTouchCount;

        private void Awake() {_inputRoutine = new RoutineHelper(this, null, InputUpdate);}

        private void InputUpdate()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray inputRay = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(inputRay, 100f);

                Tile firstHitTile = null;

                foreach(RaycastHit hit in hits)
                {
                    if(hit.transform.TryGetComponent(out firstHitTile)) break;
                }

                InputEvents.MouseDownGrid?.Invoke(firstHitTile, inputRay.origin + inputRay.direction);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                Ray inputRay = Camera.ScreenPointToRay(Input.mousePosition);
                
                InputEvents.MouseUpGrid?.Invoke(inputRay.origin + inputRay.direction);
            }

            int touchCount = Input.touchCount;
            
            if(touchCount > 1)
            {
                Touch touch1 = Input.GetTouch(0);

                Touch touch2 = Input.GetTouch(1);

                float currDist = (touch1.position - touch2.position).magnitude;

                if(_lastTouchCount < 2)
                {
                    _lastTouchCount = touchCount;
                    
                    _lastDist = currDist;
                    return;
                }

                float distDelta = _lastDist - currDist;

                if(distDelta.Abs() >= ZoomDeltaThreshold)
                {
                    InputEvents.ZoomDelta?.Invoke(distDelta);
                }

                _lastTouchCount = touchCount;
                _lastDist = currDist;
            }
            else
            {
                _lastTouchCount = 0;
                _lastDist = 0f;
            }
        }

        protected override void RegisterEvents()
        {
            GridEvents.InputStart += OnInputStart;
            GridEvents.InputStop += OnInputStop;
        }

        private void OnInputStop()
        {
            _inputRoutine.StopCoroutine();
        }

        private void OnInputStart()
        {
            _inputRoutine.StartCoroutine();
        }

        protected override void UnRegisterEvents()
        {
            GridEvents.InputStart -= OnInputStart;
            GridEvents.InputStop -= OnInputStop;
        }
    }
}