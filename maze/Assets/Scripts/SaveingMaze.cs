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

        for (int j = 0; j < maze._height; j++) 
        {
            for (int i = 0; i < maze._width; i++) 
            {
                tileData.Add(new TileData());
                Tile tile = maze._tiles[i][j];
                int index = (j * maze._width) + i;
                tileData[index].color = tile._render.color;
                tileData[index].tileType = tile.TileType;
                tileData[index].tileCost = 1;
                print(index);
            }
        }

        MazeData mazeData = new MazeData
        {
            _width = maze._width,
            _height = maze._height,
            tileSize = maze.tileSize,
            startTileX = maze.startTile != null ? maze.startTile.x : -1,
            startTileY = maze.startTile != null ? maze.startTile.y : -1,
            endTileX = maze.endTile != null ? maze.endTile.x : -1,
            endTileY = maze.endTile != null ? maze.endTile.y : -1,
            tileData = tileData
        };

        string json = JsonUtility.ToJson(mazeData, true); 
        string filePath = Application.persistentDataPath + "/MazeData.json";
        File.WriteAllText(filePath, json);

        Debug.Log("Data saved to " + filePath);
    }
}
