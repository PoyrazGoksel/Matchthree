using System.Linq;
using Events;
using Extensions.Unity;
using UnityEngine;
using Zenject;

namespace Components
{
    public class InputListener : MonoBehaviour
    {
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private Camera Camera{get;set;}
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
    }
}