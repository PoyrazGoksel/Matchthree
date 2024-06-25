using Events;
using Extensions.Unity;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using Zenject;

namespace Components
{
    public class InputListener : EventListenerMono
    {
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private Camera Camera{get;set;}
        [Inject] private GridEvents GridEvents{get;set;}
        private RoutineHelper _inputRoutine;

        private void Awake() {_inputRoutine = new RoutineHelper(this, null, InputUpdate);}

        private void Start() {_inputRoutine.StartCoroutine();}

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