using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SetTileType : MonoBehaviour
{
    public TileTypes TileType;
    Maze maze;

    private void Start()
    {
        maze = GameObject.FindFirstObjectByType<Maze>();
    }

    private void UpdateTileType(Tile tile, TileTypes newType)
    {
        if (maze.startTile == tile)
            maze.startTile = null;

        if (maze.endTile == tile)
            maze.endTile = null;

        tile.SetType(newType, maze.IsOffset(new Vector2Int(tile.x, tile.y)));
    }

    private void SetStartTile(Tile tile)
    {
        if (maze.startTile != null)
            UpdateTileType(maze.startTile, TileTypes.WALKABLE);

        UpdateTileType(tile, TileTypes.START);
        maze.startTile = tile;
    }

    private void SetEndTile(Tile tile)
    {
        if (maze.endTile != null)
            UpdateTileType(maze.endTile, TileTypes.WALKABLE);

        UpdateTileType(tile, TileTypes.END);
        maze.endTile = tile;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.tag == "tile")
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();

                if (TileType == TileTypes.WALL || TileType == TileTypes.WALKABLE)
                {
                    UpdateTileType(tile, TileType);
                }
                else if (TileType == TileTypes.START)
                {
                    SetStartTile(tile); 
                }
                else if (TileType == TileTypes.END)
                {
                    SetEndTile(tile);
                }
            }
        }
    }

    public void selectTile(int _tileNum)
    {
        if (_tileNum == 0)
            TileType = TileTypes.WALL;
        else if (_tileNum == 1)
            TileType = TileTypes.WALKABLE;
        else if (_tileNum == 2) 
            TileType = TileTypes.START;
        else if (_tileNum == 3)
            TileType = TileTypes.END;
    }
}