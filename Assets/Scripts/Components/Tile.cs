using System;
using DG.Tweening;
using Extensions.Unity;
using UnityEngine;

namespace Components
{
    public class Tile : MonoBehaviour, ICoordSet, IPoolObj
    {
        public Vector2Int Coords => _coords;
        public int ID => _id;
        [SerializeField] private Vector2Int _coords;
        [SerializeField] private int _id;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _transform;
        public MonoPool MyPool{get;set;}

        private void OnMouseDown() {}

        void ICoordSet.SetCoord(Vector2Int coord)
        {
            _coords = coord;
        }

        void ICoordSet.SetCoord(int x, int y)
        {
            _coords = new Vector2Int(x, y);
        }

        public void AfterCreate() {}

        public void BeforeDeSpawn() {}

        public void TweenDelayedDeSpawn(Func<bool> onComplete) {}

        public void AfterSpawn()
        {
            //RESET METHOD (Resurrect)
        }

        public void Construct(Vector2Int coords) {_coords = coords;}

        public void DoMove(Vector3 worldPos)
        {
            _transform.DOMove(worldPos, 1f);
        }
    }

    public interface ICoordSet
    {
        void SetCoord(Vector2Int coord);
        void SetCoord(int x, int y);
    }
}