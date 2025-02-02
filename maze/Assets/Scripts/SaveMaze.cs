using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveMaze : MonoBehaviour
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

        for (int i = 0; i < _width; i++)
        {
            var row = new List<Tile>();

            for (int j = 0; j < _height; j++)
            {
                tileData.Add(new TileData());
                Tile tile = maze._tiles[0][0];
                tileData[i + j].color = tile._render.color;
                tileData[i + j].tileType = tile.TileType;
                tileData[i + j].tileCost = 1;
            }
        }

        MazeData mazeData = new MazeData
        {
            _width = maze._width,
            _height = maze._height,
            tileSize = maze.tileSize,
            tileData = tileData
        };

        // Convert to JSON
        string json = JsonUtility.ToJson(mazeData, true); 

        // Save to file
        string filePath = Application.persistentDataPath + "/MazeData.json";
        File.WriteAllText(filePath, json);

        Debug.Log("Data saved to " + filePath);
    }
}
