using System.Collections.Generic;
using System.Linq;
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

        [Button]
        private void TestGridDir(Vector2 input) {Debug.LogWarning(GridF.GetGridDir(input));}

        [Button]
        private void TestGameOver()
        {
            bool isGameOver = IsGameOver(out Tile hintTile, out GridDir hintDir);

            Debug.LogWarning($"isGameOver: {isGameOver}, hintTile {hintTile}, hintDir {hintDir}", hintTile);
        }
        
        private void OnDrawGizmos()
        {
            if(_lastMatches == null) return;

            if(_lastMatches.Count == 0) return;
            
            Gizmos.color = Color.blue;
            
            foreach(Tile tile in _lastMatches.SelectMany(e => e))
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

            foreach(GameObject border in _gridBorders)
            {
                Bounds spriteBounds = border.GetComponentInChildren<SpriteRenderer>().bounds;
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
            
            GenerateTileBG();
            GenerateBorders();
            CalculateBounds();
        }

        
        [Button]
        private void GenerateTileBG()
        {
            _tileBGs.DoToAll(DestroyImmediate);
            _tileBGs = new List<GameObject>();
            
            foreach(Tile tile in _grid)
            {
                Vector3 tileWorldPos = tile.transform.position;

                GameObject tileBg = PrefabUtility.InstantiatePrefab
                (
                    _tileBGPrefab,
                    _bGTrans
                ) as GameObject;

                tileBg.transform.position = tileWorldPos;
                
                _tileBGs.Add(tileBg);
            }
        }

        [Button]
        private void GenerateBorders()
        {
            _gridBorders.DoToAll(DestroyImmediate);
            _gridBorders = new List<GameObject>();

            Tile botLeftCorner = _grid[0,0];
            InstantiateBorder(botLeftCorner.transform.position, _borderBotLeft);
            Tile topRightCorner = _grid[_grid.GetLength(0) - 1, _grid.GetLength(1) - 1];
            InstantiateBorder(topRightCorner.transform.position, _borderTopRight);
            Tile botRightCorner = _grid[_grid.GetLength(0) - 1,0];
            InstantiateBorder(botRightCorner.transform.position, _borderBotRight);
            Tile topLeftCorner = _grid[0,_grid.GetLength(1) - 1];
            InstantiateBorder(topLeftCorner.transform.position, _borderTopLeft);
            
            for(int x = 0; x < _grid.GetLength(0); x ++)
            {
                Tile tileBot = _grid[x, 0];
                Tile tileTop = _grid[x, _grid.GetLength(1) - 1];
                
                InstantiateBorder(tileBot.transform.position, _borderBot);
                InstantiateBorder(tileTop.transform.position, _borderTop);
            }
            for(int y = 0; y < _grid.GetLength(1); y ++)
            {
                Tile tileLeft = _grid[0, y];
                Tile tileRight = _grid[_grid.GetLength(0) - 1, y];
                
                InstantiateBorder(tileLeft.transform.position, _borderLeft);
                InstantiateBorder(tileRight.transform.position, _borderRight);
            }
        }

        private void InstantiateBorder(Vector3 tileWPos, GameObject borderPrefab)
        {
            GameObject newBorder = PrefabUtility.InstantiatePrefab
            (
                borderPrefab,
                _borderTrans
            ) as GameObject;

            newBorder.transform.position = tileWPos;
            
            _gridBorders.Add(newBorder);
        }
#endif
    }
}