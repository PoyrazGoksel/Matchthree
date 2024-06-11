using System.Collections.Generic;
using Extensions.System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Components
{
    public partial class GridManager
    {
#if UNITY_EDITOR
        private Tile DrawTile(Rect rect, Tile tile)
        {
            if(tile == false) return tile;
            
            Texture2D preview = AssetPreview.GetAssetPreview(tile.gameObject);
            
            rect = rect.Padding(3);
            EditorGUI.DrawPreviewTexture(rect, preview);
            
            return tile;
        }

        private void OnDrawGizmos()
        {
            if(_currMatchesDebug == null) return;

            if(_currMatchesDebug.Count == 0) return;
            
            Gizmos.color = Color.blue;
            
            foreach(Tile tile in _currMatchesDebug)
            {
                if(! tile) continue;

                Gizmos.DrawWireCube(tile.transform.position, Vector3.one);
            }
        }

        [Button]
        private void CalculateBounds()
        {
            _gridBounds = new Bounds();
            
            foreach(Tile tile in _grid)
            {
                 Bounds spriteBounds = tile.GetComponent<SpriteRenderer>().bounds;
                 _gridBounds.Encapsulate(spriteBounds);
            }
        }

        [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {
            _prefabIds = new List<int>();

            for(int id = 0; id < _tilePrefabs.Count; id ++) _prefabIds.Add(id);
            
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
                List<int> spawnableIds = new(_prefabIds);
                Vector2Int coord = new(x, _gridSizeY - y - 1); //Invert Y Axis
                Vector3 pos = new(coord.x, coord.y, 0f);

                _grid.GetSpawnableColors(coord, spawnableIds);
                
                int randomId = spawnableIds.Random();
                
                GameObject tilePrefabRandom = _tilePrefabs[randomId];
                GameObject tileNew = PrefabUtility.InstantiatePrefab(tilePrefabRandom, transform) as GameObject; //Instantiate rand prefab
                tileNew.transform.position = pos;
                
                Tile tile = tileNew.GetComponent<Tile>();
                tile.Construct(coord);
                
                _grid[coord.x, coord.y] = tile;// Becarefull while assigning tile to inversed y coordinates!
            }
            
            CalculateBounds();
        }
#endif
    }
}