using UnityEngine;

namespace Components
{
    public class Tile : MonoBehaviour, ICoordSet
    {
        public Vector2Int Coords => _coords;
        public int ID => _id;
        [SerializeField] private Vector2Int _coords;
        [SerializeField] private int _id;

        public void Construct(Vector2Int coords) {_coords = coords;}

        private void OnMouseDown() {}

        void ICoordSet.SetCoord(Vector2Int coord)
        {
            _coords = coord;
        }

        void ICoordSet.SetCoord(int x, int y)
        {
            _coords = new Vector2Int(x, y);
        }
    }

    public interface ICoordSet
    {
        void SetCoord(Vector2Int coord);
        void SetCoord(int x, int y);
    }
}