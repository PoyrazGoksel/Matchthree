using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
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
        [SerializeField] private int _gridSizeX;
        [SerializeField] private int _gridSizeY;
        [SerializeField] private List<int> _prefabIds;
        [SerializeField] private Bounds _gridBounds;
        [SerializeField] private Transform _transform;
        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
        private List<Tile> _currMatchesDebug;
        public ITweenContainer TweenContainer{get;set;}
        private List<MonoPool> _tilePoolsByPrefabID;
        private MonoPool _tilePool0;
        private MonoPool _tilePool1;
        private MonoPool _tilePool2;
        private MonoPool _tilePool3;
        private Tile[,] _tilesToMove;

        private void Awake()
        {
            _tilePoolsByPrefabID = new List<MonoPool>();
            
            for(int prefabId = 0; prefabId < _prefabIds.Count; prefabId ++)
            {
                MonoPool tilePool = new
                (
                    new MonoPoolData
                    (
                        _tilePrefabs[prefabId],
                        10,
                        _transform
                    )
                );
                
                _tilePoolsByPrefabID.Add(tilePool);
            }

            TweenContainer = TweenContain.Install(this);
        }

        private void Start() {GridEvents.GridLoaded?.Invoke(_gridBounds);}

        private void OnEnable() {RegisterEvents();}

        private void OnDisable()
        {
            UnRegisterEvents();
            TweenContainer.Clear();
        }

        private bool CanMove(Vector2Int tileMoveCoord) => _grid.IsInsideGrid(tileMoveCoord);

        private bool HasMatch(Tile fromTile, Tile toTile, out List<Tile> matches)
        {
            bool hasMatches = false;

            matches = _grid.GetMatchesYAll(toTile);
            matches.AddRange(_grid.GetMatchesXAll(toTile));

            matches.AddRange(_grid.GetMatchesYAll(fromTile));
            matches.AddRange(_grid.GetMatchesXAll(fromTile));

            if(matches.Count > 2) hasMatches = true;

            return hasMatches;
        }

        private bool IsGameOver(out Tile hintTile, out GridDir hintDir)
        {
            hintDir = GridDir.Null;
            hintTile = null;
            
            List<Tile> matches = new();
            
            foreach(Tile fromTile in _grid)
            {
                bool isPearl = false;

                hintTile = fromTile;

                if(hintTile.Coords == new Vector2Int(1, 0))
                {
                    Debug.LogWarning("Break");
                    isPearl = true;
                }
                
                Vector2Int thisCoord = fromTile.Coords;

                Vector2Int leftCoord = thisCoord + Vector2Int.left;
                Vector2Int topCoord = thisCoord + Vector2Int.up;
                Vector2Int rightCoord = thisCoord + Vector2Int.right;
                Vector2Int botCoord = thisCoord + Vector2Int.down;

                if(_grid.IsInsideGrid(leftCoord))
                {
                    Tile toTile = _grid.Get(leftCoord);

                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile, isPearl));

                    if(isPearl)
                    {
                        Debug.LogWarning($"fromTile.Coords: {fromTile.Coords}, leftCoord {leftCoord}");
                        Debug.LogWarning($"matches{matches.Count}");
                    }

                    _grid.Swap(toTile, fromTile);

                    if(matches.Count > 0)
                    {
                        hintDir = GridDir.Left;
                        return false;
                    }
                }
                
                if(_grid.IsInsideGrid(topCoord))
                {
                    if(isPearl)
                    {
                        Debug.LogWarning(topCoord);
                    }
                    Tile toTile = _grid.Get(topCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));
                    
                    _grid.Swap(toTile, fromTile);
                    
                    if(matches.Count > 0)
                    {
                        hintDir = GridDir.Up;
                        return false;
                    }
                }
                
                if(_grid.IsInsideGrid(rightCoord))
                {
                    if(isPearl) { Debug.LogWarning(rightCoord); }

                    Tile toTile = _grid.Get(rightCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));
                    
                    _grid.Swap(toTile, fromTile);
                    
                    if(matches.Count > 0)
                    {
                        hintDir = GridDir.Right;
                        return false;
                    }
                }
                
                if(_grid.IsInsideGrid(botCoord))
                {
                    if(isPearl) { Debug.LogWarning(botCoord); }

                    Tile toTile = _grid.Get(botCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));
                    
                    _grid.Swap(toTile, fromTile);
                    
                    if(matches.Count > 0)
                    {
                        hintDir = GridDir.Down;
                        return false;
                    }
                }
            }

            return matches.Count == 0;
        }

        [Button]
        private void TestGridDir(Vector2 input) {Debug.LogWarning(GridF.GetGridDir(input));}

        [Button]
        private void TestGameOver()
        {
            bool isGameOver = IsGameOver(out Tile hintTile, out GridDir hintDir);

            Debug.LogWarning($"isGameOver: {isGameOver}, hintTile {hintTile}, hintDir {hintDir}", hintTile);
        }

        private void RainDownTiles()
        {
            bool didDestroy = true;

            _tilesToMove = new Tile[_gridSizeX,_gridSizeY];

            for(int y = 0; y < _gridSizeY; y ++)
            for(int x = 0; x < _gridSizeX; x ++)
            {
                Vector2Int thisCoord = new(x, y);
                Tile thisTile = _grid.Get(thisCoord);

                if(thisTile) continue;

                int spawnPoint = _gridSizeY;

                for(int y1 = y; y1 <= spawnPoint; y1 ++)
                {
                    if(y1 == spawnPoint)
                    {
                        MonoPool randomPool = _tilePoolsByPrefabID.Random();
                        Tile newTile = randomPool.Request<Tile>();
                        
                        Vector3 spawnWorldPos = _grid.CoordsToWorld(_transform, new Vector2Int(x, spawnPoint));
                        newTile.Teleport(spawnWorldPos);
                        
                        _grid.Set(newTile, thisCoord);
                        
                        _tilesToMove[thisCoord.x, thisCoord.y] = newTile;
                        break;
                    }

                    Vector2Int emptyCoords = new(x, y1);

                    Tile mostTopTile = _grid.Get(emptyCoords);

                    if(mostTopTile)
                    {
                        _grid.Set(null, mostTopTile.Coords);
                        _grid.Set(mostTopTile, thisCoord);
                        
                        _tilesToMove[thisCoord.x, thisCoord.y] = mostTopTile;

                        break;
                    }
                }
            }

            StartCoroutine(RainDownRoutine());
        }

        private IEnumerator RainDownRoutine()
        {
            for(int y = 0; y < _gridSizeY; y ++) // TODO: Should start from first tile that we are moving
            {
                for(int x = 0; x < _gridSizeX; x ++)
                {
                    Tile thisTile = _tilesToMove[x, y];

                    if(thisTile == false) continue;

                    thisTile.DoMove(_grid.CoordsToWorld(_transform, thisTile.Coords));
                }

                yield return new WaitForSecondsRealtime(0.1f);
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
                        matches.DoToAll
                        (
                            e =>
                            {
                                _grid.Set(null, e.Coords);
                                e.gameObject.Destroy();
                            }
                        );

                        RainDownTiles();
                    }
                );

                _currMatchesDebug = matches;
            }
        }

        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
        }
    }
}