using System.Collections.Generic;
using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Components
{
    public partial class GridManager : SerializedMonoBehaviour, ITweenContainerBind
    {
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private GridEvents GridEvents{get;set;}
        [BoxGroup(Order = 999)]
        [TableMatrix(SquareCells = true, DrawElementMethod = nameof(DrawTile))]
        [OdinSerialize]
        private Tile[,] _grid;
        [SerializeField] private List<GameObject> _tilePrefabs;
        private int _gridSizeX;
        private int _gridSizeY;
        [SerializeField] private List<int> _prefabIds;
        [SerializeField] private Bounds _gridBounds;
        [SerializeField] private Transform _transform;
        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
        private List<Tile> _currMatchesDebug;
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            TweenContainer = TweenContain.Install(this);
        }

        private void Start() {GridEvents.GridLoaded?.Invoke(_gridBounds);}

        private void OnEnable() {RegisterEvents();}

        private void OnDisable()
        {
            UnRegisterEvents();
            TweenContainer.Clear();
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

        private bool CanMove(Vector2Int tileMoveCoord)
        {
            return _grid.IsInsideGrid(tileMoveCoord);
        }

        private bool HasMatch(Tile fromTile, Tile toTile, out List<Tile> matches)
        {
            bool hasMatches = false;
            
            matches = _grid.GetMatchesY(toTile);
            matches.AddRange(_grid.GetMatchesX(toTile));

            matches.AddRange(_grid.GetMatchesY(fromTile));
            matches.AddRange(_grid.GetMatchesX(fromTile));

            if(matches.Count > 2) hasMatches = true;
            
            return hasMatches;
        }

        [Button]
        private void TestGridDir(Vector2 input) {Debug.LogWarning(GridF.GetGridDir(input));}

        private void OnMouseUpGrid(Vector3 mouseUpPos)
        {
            _mouseUpPos = mouseUpPos;

            Vector3 dirVector = mouseUpPos - _mouseDownPos;

            if(_selectedTile)
            {
                Vector2Int tileMoveCoord = _selectedTile.Coords + GridF.GetGridDirVector(dirVector);

                if(! CanMove(tileMoveCoord)) return;

                Tile toTile = _grid.Get(tileMoveCoord);
                
                _grid.Swap(_selectedTile, toTile);

                if(! HasMatch(_selectedTile, toTile, out List<Tile> matches))
                {
                    _grid.Swap(toTile, _selectedTile);
                    return;
                }
                
                DoTileMoveAnim
                (
                    _selectedTile,
                    toTile,
                    delegate
                    {
                        matches.DoToAll(e => e.gameObject.Destroy());
                    }
                );
                
                _currMatchesDebug = matches;
            }
        }

        private void DoTileMoveAnim(Tile fromTile, Tile toTile, TweenCallback onComplete)
        {
            TweenContainer.AddSequence = DOTween.Sequence();
            Vector3 fromTileWorldPos = _grid.CoordsToWorld(_transform, fromTile.Coords);
            TweenContainer.AddedSeq.Append(fromTile.transform.DOMove(fromTileWorldPos, 1f));
            Vector3 toTileWorldPos = _grid.CoordsToWorld(_transform, toTile.Coords);
            TweenContainer.AddedSeq.Join(toTile.transform.DOMove(toTileWorldPos, 1f));
            
            TweenContainer.AddedSeq.onComplete += onComplete;
        }
        
        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
        }
    }
}