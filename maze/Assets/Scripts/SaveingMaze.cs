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
    public List<SpriteRenderer> Sprites;

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

        for (int j = 0; j < maze.height; j++) 
        {
            for (int i = 0; i < maze.width; i++) 
            {
                tileData.Add(new TileData());
                Tile tile = maze._tiles[i][j];
                int index = (j * maze.width) + i;
                tileData[index].color = tile._render.color;
                tileData[index].tileType = tile.TileType;
                tileData[index].tileCost = 1;
            }
        }

        MazeData mazeData = new MazeData
        {
            _width = maze.width,
            _height = maze.height,
            tileSize = maze.tileSize,
            startTileX = maze.startTile != null ? maze.startTile.x : -1,
            startTileY = maze.startTile != null ? maze.startTile.y : -1,
            endTileX = maze.endTile != null ? maze.endTile.x : -1,
            endTileY = maze.endTile != null ? maze.endTile.y : -1,
            tileData = tileData
        };

        Sprites = new List<SpriteRenderer>();
        
        for (int i = 0;i < maze.width; i++)
        {
            for (int j = 0; j < maze.height; j++)
            {
                Sprites.Add(maze._tiles[i][j].GetComponent<SpriteRenderer>());
            }
        }

        string json = JsonUtility.ToJson(mazeData, true); 
        string filePath = Application.persistentDataPath + "/MazeData.json";
        File.WriteAllText(filePath, json);
        SpriteToImage.CaptureAndSaveImage(spriteRenderers: Sprites, Application.persistentDataPath , fileName: "my_image.png");

        Debug.Log("Data saved to " + filePath);
    }
}
