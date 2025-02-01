using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Maze : MonoBehaviour, IGame
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private MyButton _btnPrefab;

    public List<List<Tile>> _tiles;
    

    [SerializeField] private Transform _cam;
    public Tile startTile, endTile;
    public List<GridStep> _steps = new List<GridStep>();
    float tileSize;
    float longestDimension;

    void Start()
    {
        GenerateGrid();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid();
        }


    }


    void GenerateGrid()
    {
        _tiles = new List<List<Tile>>();
        _steps = new List<GridStep>();

        _steps.Add(new GridStep(DIRECTION.UP));
        _steps.Add(new GridStep(DIRECTION.LEFT));
        _steps.Add(new GridStep(DIRECTION.RIGHT));
        _steps.Add(new GridStep(DIRECTION.DOWN));

        startTile = endTile = null;

        DeleteAllTiles();
        UpdateMazeSize();

        for (int i = 0; i < longestDimension; i++)
        {
            var row = new List<Tile>();

            for (int j = 0; j < _height; j++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(i* tileSize, j* tileSize), Quaternion.identity);
                spawnedTile.transform.parent = gameObject.transform;
                spawnedTile.transform.localScale = new Vector2(tileSize, tileSize);

                spawnedTile.name = $"Tile {i} {j}";

                spawnedTile.InitState(i, j);
              
                spawnedTile.SetType(TileTypes.WALKABLE, IsOffset(new Vector2Int(i, j)));

                row.Add(spawnedTile);
            }
            // Add the row to the grid
            _tiles.Add(row);
        }

        _cam.transform.position = new Vector3((_width * tileSize) /2 - (0.5f * tileSize), (_height * tileSize) / 2 - (0.5f * tileSize), -10);
    }

    public IState getInitState()
    {
        return _tiles[startTile.x][startTile.y];
    }

    void DeleteAllTiles()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }

    public bool isGoal(IState state)
    {
        if (state is Tile tile)
        {
            // Goal is bottom-right tile
            return tile.x == endTile.x && tile.y == endTile.y;
        }
        return false;
    }

    public bool IsOffset(Vector2Int _tilePosition)
    {
        var isOffset = (_tilePosition.x % 2 == 0 && _tilePosition.y % 2 != 0) || (_tilePosition.x % 2 != 0 && _tilePosition.y % 2 == 0);
        return isOffset;
    }

    public float getCost(IState state = null, IStep step  = null)
    {
        return 1.0f;
    }

    public List<IStep> getSteps(IState state)
    {
        var steps = new List<IStep>();

        if (state is Tile currentTile)
        {
            foreach (var step in _steps)
            {
                int newX = currentTile.x + step.dx;
                int newY = currentTile.y + step.dy;

                // Check if the new position is within grid boundaries
                if (newX >= 0 && newX < _width && newY >= 0 && newY < _height)
                {
                    var targetTile = _tiles[newX][newY];

                    // Check if the target tile is walkable
                    if (targetTile.TileType != TileTypes.WALL)
                    {
                        steps.Add(step);
                    }
                }
            }
        }

        return steps;
    }

    public IState getSuccessor(IState state, IStep step)
    {
        if (state is Tile currentTile && step is GridStep gridStep)
        {
            int newX = currentTile.x + gridStep.dx;
            int newY = currentTile.y + gridStep.dy;

            return _tiles[newX][newY];
        }

        return null;
    }

    public IEnumerator AnimatePath(List<IState> exploredNodes, List<IStep> pathSteps)
    {
        // Animate explored nodes
        foreach (var state in exploredNodes)
        {
            if (state is Tile tile)
            {
                tile.HighlightAsVisited(); // Highlight the explored tile
                yield return new WaitForSeconds(0.1f); // Delay for animation
            }
        }

        // Animate path
        var currentTile = getInitState() as Tile;
        foreach (var step in pathSteps)
        {
            if (currentTile != null)
            {
                currentTile.HighlightAsPath();
                yield return new WaitForSeconds(0.2f); // Animation delay
            }

            currentTile = getSuccessor(currentTile, step) as Tile;
        }
    }

    public void StartSearch(IAlgorithm algorithm)
    {
        var initialState = getInitState();
        var result = algorithm.Search(this, initialState);

        var exploredNodes = result.Item2; // Explored nodes
        var pathSteps = result.Item1; // Path to goal

        StartCoroutine(AnimatePath(exploredNodes, pathSteps));
    }

    public void UpdateMazeSize()
    {
        Math.Clamp(_height, 5, 100);
        Math.Clamp(_width, 5, 100);

        if (Math.Abs(_height - _width) > 5)
        {
            if (_width > _width)
            {
                _width = _height - 5;
            }
            else
            {
                _height = _width - 5;
            }
        }

        longestDimension = Math.Max(_height, _width);

        tileSize = 5f / longestDimension;
    }
}


