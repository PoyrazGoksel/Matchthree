using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Components
{
    public class GridManager : SerializedMonoBehaviour
    {
        [BoxGroup(Order = 999)][TableMatrix(SquareCells = true)/*(DrawElementMethod = nameof(DrawTile))*/,OdinSerialize]
        private Tile[,] _grid;
        [SerializeField] private List<GameObject> _tilePrefabs;

        private int _gridSizeX;
        private int _gridSizeY;
        
        private Tile DrawTile(Rect rect, Tile tile)
        {
            UnityEditor.EditorGUI.DrawRect(rect, Color.blue);
            
            return tile;
        }

        [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {
            _gridSizeX = sizeX;
            _gridSizeY = sizeY;
            
            if(_grid != null)
            {
                foreach(Tile o in _grid)
                {
                    DestroyImmediate(o.gameObject);
                }
            }

            _grid = new Tile[_gridSizeX, _gridSizeY];

            for(int x = 0; x < _gridSizeX; x ++)
            for(int y = 0; y < _gridSizeY; y ++)
            {
                Vector2Int coord = new(x, _gridSizeY - y - 1);
                Vector3 pos = new(coord.x, coord.y, 0f);
                int randomIndex = Random.Range(0, _tilePrefabs.Count);
                GameObject tilePrefabRandom = _tilePrefabs[randomIndex];
                GameObject tileNew = Instantiate(tilePrefabRandom, pos, Quaternion.identity);
                Tile tile = tileNew.GetComponent<Tile>();
                tile.Construct(coord);
                _grid[x, y] = tile;
            }
        }
    }
}