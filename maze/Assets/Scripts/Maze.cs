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

    void Start()
    {
        _tiles = new List<List<Tile>>();


        _steps.Add(new GridStep(DIRECTION.UP));
        _steps.Add(new GridStep(DIRECTION.LEFT));
        _steps.Add(new GridStep(DIRECTION.RIGHT));
        _steps.Add(new GridStep(DIRECTION.DOWN));


        //var btn1 = Instantiate(_btnPrefab, new Vector3(0, 0), Quaternion.identity);

        GenerateGrid();

        //StartCoroutine(AnimateSearch());
       // StartSearch(new BreadthFS());
    }

    void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            var row = new List<Tile>();

            for (int j = 0; j < _height; j++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(i, j), Quaternion.identity);
                spawnedTile.transform.parent = gameObject.transform;
                
                spawnedTile.name = $"Tile {i} {j}";

                spawnedTile.InitState(i, j);
              
                spawnedTile.SetType(TileTypes.WALKABLE, IsOffset(new Vector2Int(i, j)));

                // set StartPoint & EndPoint & Walkable
                if (i == 0 && j == 0)
                {
                   // spawnedTile.SetType(TileTypes.START, isOffset);
                }
                else if (i == _width - 1 && j == _height - 1)
                {
                   // spawnedTile.SetType(TileTypes.END, isOffset);
                }
                else if(i == 4 && j < 5)
                {
                  //  spawnedTile.SetType(TileTypes.WALL, isOffset);
                }
                else if (j == 4 && i < 3)
                {
                  //  spawnedTile.SetType(TileTypes.WALL, isOffset);
                }
                else 
                {
                
                }

                // Add the tile to the current row
                row.Add(spawnedTile);
            }
            // Add the row to the grid
            _tiles.Add(row);
        }

        _cam.transform.position = new Vector3((float)_width/2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    public IState getInitState()
    {
        return _tiles[startTile.x][startTile.y];
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
}
