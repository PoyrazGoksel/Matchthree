using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using Zenject;

namespace Components
{
    public class CameraSizeFitter : EventListenerMono
    {
        [Inject] private GridEvents GridEvents{get;set;}
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _transform;
        
        protected override void RegisterEvents()
        {
            GridEvents.GridLoaded += OnGridLoaded;
        }

        private void OnGridLoaded(Bounds gridBounds)
        {
            _transform.position = gridBounds.center + (Vector3.back * 10f);
            _camera.orthographicSize = gridBounds.extents.x * (1f / _camera.aspect);
        }

        protected override void UnRegisterEvents()
        {
            GridEvents.GridLoaded -= OnGridLoaded;
        }
    }
}