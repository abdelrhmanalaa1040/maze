using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveingMaze : MonoBehaviour
{
    public int _width, _height;
    public float tileSize;
    public List<TileData> tileData;
    public Maze maze;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveToJson();
        }
    }

    public void SaveToJson()
    {
        tileData = new List<TileData>();

        for (int i = 0; i < maze._width; i++)
        {
            var row = new List<Tile>();
            for (int j = 0; j < maze._height; j++)
            {
                tileData.Add(new TileData());
                Tile tile = maze._tiles[i][j];
                tileData[(i * maze._width) + j].color = tile._render.color;
                tileData[(i * maze._width) + j].tileType = tile.TileType;
                tileData[(i * maze._width) + j].tileCost = 1;
                print((i * maze._width) + j);
            }
        }

        MazeData mazeData = new MazeData
        {
            _width = maze._width,
            _height = maze._height,
            tileSize = maze.tileSize,
            startTileX = maze.startTile.x,
            startTileY = maze.startTile.y,
            endTileX = maze.endTile.x,
            endTileY = maze.endTile.y,
            tileData = tileData
        };

        string json = JsonUtility.ToJson(mazeData, true); 
        string filePath = Application.persistentDataPath + "/MazeData.json";
        File.WriteAllText(filePath, json);

        Debug.Log("Data saved to " + filePath);
    }
}
