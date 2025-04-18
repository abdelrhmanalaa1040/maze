using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEngine.UI;


public class Maze : MonoBehaviour, IGame
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private MyButton _btnPrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private bool useAnimationDelay = true;
    [SerializeField] private int stepLimit;

    public float tileAnimationSpeed = 0.2f;
    public Slider stepsSlider;

    public int _width, _height;
    [HideInInspector] public List<List<Tile>> _tiles;

    public Tile startTile, endTile;
    [HideInInspector] public List<GridStep> _steps = new List<GridStep>();

    public Strategy strategy;

    [HideInInspector] public float tileSize;
    float longestDimension;

    int previousHeight;
    int previousWidth;

    void Start()
    {
        GenerateGrid(tileAnimationSpeed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid(tileAnimationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RefreshTiles();
        }

        if (previousHeight != _height || previousWidth != _width)
        {
            GenerateGrid(tileAnimationSpeed);
            previousHeight = _height;
            previousWidth = _width;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            print("start");
            selectingAlgorithm(false);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFromJson();
        }
    }

    void selectingAlgorithm(bool _addLimit)
    {
        if (strategy == Strategy.DFS)
        {
            StartSearch(new DepthFirstSearch(), _addLimit);
        }
        else if (strategy == Strategy.BFS)
        {
            StartSearch(new BreadthFS(), _addLimit);
        }
        else if (strategy == Strategy.Dijkstra)
        {
            StartSearch(new Dijkstra(), _addLimit);
        }

    }

    void GenerateGrid(float _tileAnimationSpeed)
    {
        print("new maze");
        _tiles = new List<List<Tile>>();
        _steps = new List<GridStep>();
        previousHeight = _height;
        previousWidth = _width;
        _steps.Add(new GridStep(DIRECTION.DOWN));
        _steps.Add(new GridStep(DIRECTION.LEFT));
        _steps.Add(new GridStep(DIRECTION.UP));
        _steps.Add(new GridStep(DIRECTION.RIGHT));

        startTile = endTile = null;

        DeleteAllTiles();
        UpdateMazeSize();

        for (int i = 0; i < _width; i++)
        {
            var row = new List<Tile>();

            for (int j = 0; j < _height; j++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(i * tileSize, j * tileSize), Quaternion.identity);
                spawnedTile.transform.parent = gameObject.transform;
                spawnedTile.transform.localScale = new Vector3(tileSize, tileSize, transform.position.z);
                spawnedTile.GetComponent<TileAnimation>().AnimationSpeed = _tileAnimationSpeed;
                spawnedTile.name = $"Tile {i} {j}";

                spawnedTile.InitState(i, j);

                spawnedTile.SetType(TileTypes.WALKABLE, IsOffset(new Vector2Int(i, j)));

                row.Add(spawnedTile);
            }
            // Add the row to the grid
            _tiles.Add(row);
        }

        _cam.transform.position = new Vector3((_width * tileSize) / 2 - (0.5f * tileSize), (_height * tileSize) / 2 - (0.5f * tileSize), -10);
    }

    void RefreshTiles()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Tile _tile = _tiles[i][j].GetComponent<Tile>();

                _tile.SetType(_tile.TileType, IsOffset(new Vector2Int(i, j)));
            }
        }
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/MazeData.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            MazeData mazeData = JsonUtility.FromJson<MazeData>(json);

            _width = mazeData._width;
            _height = mazeData._height;
            GenerateGrid(tileAnimationSpeed);
            tileSize = mazeData.tileSize;

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    TileData data = mazeData.tileData[(j * mazeData._width) + i];
                    _tiles[i][j]._render.color = data.color;
                    _tiles[i][j].TileType = data.tileType;
                }
            }

            bool IsValid(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;

            if (IsValid(mazeData.startTileX, mazeData.startTileY))
                startTile = _tiles[mazeData.startTileX][mazeData.startTileY];

            if (IsValid(mazeData.endTileX, mazeData.endTileY))
                endTile = _tiles[mazeData.endTileX][mazeData.endTileY];

            Debug.Log("Data loaded from " + filePath);
        }
        else
        {
            Debug.LogError("No saved data found at " + filePath);
        }
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

    public float getCost(IState state = null, IStep step = null)
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

    public IEnumerator AnimatePath(List<IState> exploredNodes, List<IStep> pathSteps, int _stepLimit, bool _useAnimationDelay)
    {
        int stepCount = 0;

        // Animate explored nodes
        foreach (var state in exploredNodes)
        {
            if (stepCount >= _stepLimit)
                yield break; // Stop if step limit is reached

            if (state is Tile tile)
            {
                tile.HighlightAsVisited();
                if (_useAnimationDelay)
                {
                    yield return new WaitForSeconds(0.1f); // Delay for animation
                }
                stepCount++;
                stepsSlider.value = stepCount;
            }
        }

        // Animate path
        var currentTile = getInitState() as Tile;
        foreach (var step in pathSteps)
        {
            if (stepCount >= _stepLimit)
                yield break; // Stop if step limit is reached

            if (currentTile != null)
            {
                currentTile.HighlightAsPath();
                if (_useAnimationDelay)
                {
                    yield return new WaitForSeconds(0.2f); // Animation delay
                }
                stepCount++;
                stepsSlider.value = stepCount;
            }
            currentTile = getSuccessor(currentTile, step) as Tile;
        }
    }

    public void StartSearch(IAlgorithm algorithm, bool _addLimit)
    {
        var initialState = getInitState();
        var result = algorithm.Search(this, initialState);

        var exploredNodes = result.Item2; // Explored nodes
        var pathSteps = result.Item1; // Path to goal

        stepsSlider.maxValue = exploredNodes.Count + pathSteps.Count;
        if (_addLimit == false)
            StartCoroutine(AnimatePath(exploredNodes, pathSteps, exploredNodes.Count + pathSteps.Count, true));
        else
            StartCoroutine(AnimatePath(exploredNodes, pathSteps, (int)stepsSlider.value, false));
    }

    public void ReStartSearch()
    {
        print("ReStartSearch");
        RefreshTiles();
        selectingAlgorithm(true);
    }

    public void UpdateMazeSize()
    {
        _height = Math.Clamp(_height, 5, 100);
        _width = Math.Clamp(_width, 5, 100);

        longestDimension = Math.Max(_height, _width);

        tileSize = 8f / longestDimension;
    }
}
