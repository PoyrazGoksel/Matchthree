using System.Collections.Generic;
using System.Linq;
using Events;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Zenject;

namespace Components
{
    public partial class GridManager : SerializedMonoBehaviour
    {
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private GridEvents GridEvents{get;set;}
        [BoxGroup(Order = 999), TableMatrix(SquareCells = true, DrawElementMethod = nameof(DrawTile)),OdinSerialize] private Tile[,] _grid;
        [SerializeField] private List<GameObject> _tilePrefabs;
        private int _gridSizeX;
        private int _gridSizeY;
        [SerializeField] private List<int> _prefabIds;
        [SerializeField] private Bounds _gridBounds;
        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
        private List<Tile> _currMatchesDebug;

        private void Start()
        {
            GridEvents.GridLoaded?.Invoke(_gridBounds);
        }

        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        private void RegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
        }

        private void OnMouseDownGrid(Tile clickedTile, Vector3 dirVector)
        {
            _selectedTile = clickedTile;
            _mouseDownPos = dirVector;
        }

        private bool CanMove(Tile clickedTile, Vector3 inputVect, out List<Tile> matches)
        {
            matches = new List<Tile>();
            
            Vector2Int tileMoveCoord = clickedTile.Coords + GridF.GetGridDirVector(inputVect);

            if(_grid.IsInsideGrid(tileMoveCoord) == false) return false;

            return HasMatch(clickedTile, tileMoveCoord, out matches);
        }

        private bool HasMatch(Tile fromTile, Vector2Int tileMoveCoord, out List<Tile> matches)
        {
            bool hasMatches = false;
            
            Tile toTile = _grid.Get(tileMoveCoord);
            _grid.Switch(fromTile, toTile);

            matches = _grid.GetMatchesY(toTile);
            matches.AddRange(_grid.GetMatchesX(toTile));
            matches.AddRange(_grid.GetMatchesY(fromTile));
            matches.AddRange(_grid.GetMatchesX(fromTile));

            if(matches.Count > 2)
            {
                hasMatches = true;
            }
            
            _grid.Switch(toTile, fromTile);
            return hasMatches;
        }

        [Button]
        private void TestGridDir(Vector2 input)
        {
            Debug.LogWarning(GridF.GetGridDir(input));
        }

        private void OnMouseUpGrid(Vector3 arg0)
        {
            _mouseUpPos = arg0;

            Vector3 dirVector = arg0 - _mouseDownPos;

            if(_selectedTile)
            {
                bool canMove = CanMove(_selectedTile, dirVector, out List<Tile> matches);
                Debug.LogWarning($"{canMove} canMove, {matches.Count} matches.Count");

                if(! canMove) return;

                _currMatchesDebug = matches;
                
                Debug.DrawLine(_mouseDownPos, _mouseUpPos, Color.blue, 2f);
            }
        }

        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
        }
    }
}