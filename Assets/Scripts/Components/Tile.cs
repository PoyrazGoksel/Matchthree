using UnityEngine;

namespace Components
{
    public class Tile : MonoBehaviour
    {
        
        public Vector2Int Coords => _coords;
        public int ID => _id;
        [SerializeField] private Vector2Int _coords;
        [SerializeField] private int _id;

        public void Construct(Vector2Int coords)
        {
            _coords = coords;
        }

        private void OnMouseDown()
        {
            
        }
    }
}