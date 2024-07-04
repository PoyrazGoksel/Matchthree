using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<GameObject> _tileBGs = new();
        [SerializeField] private GameObject _tileBGPrefab;
        [SerializeField] private Transform _bGTrans;
        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
        public ITweenContainer TweenContainer{get;set;}
        private List<MonoPool> _tilePoolsByPrefabID;
        private MonoPool _tilePool0;
        private MonoPool _tilePool1;
        private MonoPool _tilePool2;
        private MonoPool _tilePool3;
        private Tile[,] _tilesToMove;
        [OdinSerialize]private List<List<Tile>> _lastMatches;
        private Tile _hintTile;
        private GridDir _hintDir;

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

        private void Start()
        {
            for(int x = 0; x < _grid.GetLength(0); x ++)
            for(int y = 0; y < _grid.GetLength(1); y ++)
            {
                Tile tile = _grid[x, y];

                SpawnTile(tile.ID, _grid.CoordsToWorld(_transform, tile.Coords), tile.Coords);
                tile.gameObject.Destroy();
            }

            IsGameOver(out _hintTile, out _hintDir);
            GridEvents.GridLoaded?.Invoke(_gridBounds);
            GridEvents.InputStart?.Invoke();
        }

        private void OnEnable() {RegisterEvents();}

        private void OnDisable()
        {
            UnRegisterEvents();
            TweenContainer.Clear();
        }

        private bool CanMove(Vector2Int tileMoveCoord) => _grid.IsInsideGrid(tileMoveCoord);

        private bool HasMatch(Tile fromTile, Tile toTile, out List<List<Tile>> matches)
        {
            matches = new List<List<Tile>>();
            bool hasMatches = false;

            List<Tile> matchesAll = _grid.GetMatchesYAll(toTile);
            matchesAll.AddRange(_grid.GetMatchesXAll(toTile));

            if(matchesAll.Count > 0)
            {
                matches.Add(matchesAll);
            }

            matchesAll = _grid.GetMatchesYAll(fromTile);
            matchesAll.AddRange(_grid.GetMatchesXAll(fromTile));

            if(matchesAll.Count > 0)
            {
                matches.Add(matchesAll);
            }
            
            if(matches.Count > 0) hasMatches = true;

            return hasMatches;
        }

        private bool HasAnyMatches(out List<List<Tile>> matches)
        {
            matches = new List<List<Tile>>();
            
            foreach(Tile tile in _grid)
            {
                List<Tile> matchesAll = _grid.GetMatchesXAll(tile);
                matchesAll.AddRange(_grid.GetMatchesYAll(tile));

                if(matchesAll.Count > 0)
                {
                    matches.Add(matchesAll);
                }
            }
            
            return matches.Count > 0;
        }

        private bool IsGameOver(out Tile hintTile, out GridDir hintDir)
        {
            hintDir = GridDir.Null;
            hintTile = null;
            
            List<Tile> matches = new();
            
            foreach(Tile fromTile in _grid)
            {
                hintTile = fromTile;

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
                    matches.AddRange(_grid.GetMatchesY(fromTile));

                    _grid.Swap(toTile, fromTile);

                    if(matches.Count > 0)
                    {
                        hintDir = GridDir.Left;
                        return false;
                    }
                }
                
                if(_grid.IsInsideGrid(topCoord))
                {
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

        private void SpawnAndAllocateTiles()
        {
            _tilesToMove = new Tile[_gridSizeX,_gridSizeY];

            for(int y = 0; y < _gridSizeY; y ++)
            {
                int spawnStartY = 0;
                
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
                            if(spawnStartY == 0)
                            {
                                spawnStartY = thisCoord.y;
                            }
                        
                            MonoPool randomPool = _tilePoolsByPrefabID.Random();
                            Tile newTile = SpawnTile
                            (
                                randomPool, 
                                _grid.CoordsToWorld(_transform, new Vector2Int(x, spawnPoint)),
                                thisCoord
                            );
                        
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
            }

            StartCoroutine(RainDownRoutine());
        }

        private Tile SpawnTile(MonoPool randomPool, Vector3 spawnWorldPos, Vector2Int spawnCoords)
        {
            Tile newTile = randomPool.Request<Tile>();

            newTile.Teleport(spawnWorldPos);
            
            _grid.Set(newTile, spawnCoords);

            return newTile;
        }

        private Tile SpawnTile(int id, Vector3 worldPos, Vector2Int coords) => SpawnTile(_tilePoolsByPrefabID[id], worldPos, coords);
        
        private IEnumerator RainDownRoutine()
        {
            int longestDistY = 0;
            Tween longestTween = null;
            
            for(int y = 0; y < _gridSizeY; y ++) // TODO: Should start from first tile that we are moving
            {
                bool shouldWait = false;
                
                for(int x = 0; x < _gridSizeX; x ++)
                {
                    Tile thisTile = _tilesToMove[x, y];

                    if(thisTile == false) continue;

                    Tween thisTween = thisTile.DoMove(_grid.CoordsToWorld(_transform, thisTile.Coords));

                    shouldWait = true;

                    if(longestDistY < y)
                    {
                        longestDistY = y;
                        longestTween = thisTween;
                    }
                }

                if(shouldWait)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if(longestTween != null)
            {
                longestTween.onComplete += delegate
                {
                    if(HasAnyMatches(out _lastMatches))
                    {
                        StartCoroutine(DestroyRoutine());
                    }
                    else
                    {
                        IsGameOver(out _hintTile, out _hintDir);
                        GridEvents.InputStart?.Invoke();
                    }
                };
            }
            else
            {
                Debug.LogWarning("This should not have happened!");
                GridEvents.InputStart?.Invoke();
            }
        }

        private IEnumerator DestroyRoutine()
        {
            foreach(List<Tile> matches in _lastMatches)
            {
                matches.DoToAll(DespawnTile);

                EDebug.Method();
                
                yield return new WaitForSeconds(0.1f);
            }
            
            SpawnAndAllocateTiles();
        }
        
        private void DespawnTile(Tile e)
        {
            _grid.Set(null, e.Coords);
            _tilePoolsByPrefabID[e.ID].DeSpawn(e);
        }

        private void DoTileMoveAnim(Tile fromTile, Tile toTile, TweenCallback onComplete = null)
        {
            Vector3 fromTileWorldPos = _grid.CoordsToWorld(_transform, fromTile.Coords);
            fromTile.DoMove(fromTileWorldPos);
            Vector3 toTileWorldPos = _grid.CoordsToWorld(_transform, toTile.Coords);
            toTile.DoMove(toTileWorldPos, onComplete);
        }

        private void TryShowHint()
        {
            if(_hintTile)
            {
                Vector2Int gridMoveDir = _hintDir.ToVector();

                Vector3 moveCoords = _grid.CoordsToWorld(_transform, _hintTile.Coords + gridMoveDir);
                
                _hintTile.DoMove(moveCoords);
            }
        }

        private void RegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
            GridEvents.InputStart += OnInputStart;
        }

        private void OnInputStart()
        {
            this.WaitFor(new WaitForSeconds(1f), TryShowHint);
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

                if(! HasMatch(_selectedTile, toTile, out _lastMatches))
                {
                    GridEvents.InputStop?.Invoke();

                    DoTileMoveAnim(_selectedTile, toTile,
                        delegate
                        {
                            _grid.Swap(toTile, _selectedTile);
                            
                            DoTileMoveAnim(_selectedTile, toTile,
                                delegate
                                {
                                    GridEvents.InputStart?.Invoke();
                                });
                        });
                }
                else
                {
                    GridEvents.InputStop?.Invoke();

                    DoTileMoveAnim
                    (
                        _selectedTile,
                        toTile,
                        delegate
                        {
                            StartCoroutine(DestroyRoutine());
                        }
                    );
                }
            }
        }
        
        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
            GridEvents.InputStart -= OnInputStart;
        }
    }
}