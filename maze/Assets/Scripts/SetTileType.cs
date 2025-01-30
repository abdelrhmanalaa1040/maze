using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SetTileType : MonoBehaviour
{
    public TileTypes TileType;
    Maze maze;

    private void Start()
    {
        maze = GameObject.FindFirstObjectByType<Maze>();
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider != null && hit.collider.gameObject.tag == "tile")
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();

                if (TileType == TileTypes.WALL)
                {
                    tile.SetType(TileTypes.WALL, maze.IsOffset(new Vector2Int(tile.x, tile.y)));
                    if (maze.startTile == tile)
                        maze.startTile = null;
                    
                    if (maze.endTile == tile)
                        maze.endTile = null;
                }
                else if (TileType == TileTypes.WALKABLE)
                {
                    tile.SetType(TileTypes.WALKABLE, maze.IsOffset(new Vector2Int(tile.x, tile.y)));
                    if (maze.startTile == tile)
                        maze.startTile = null;

                    if (maze.endTile == tile)
                        maze.endTile = null;

                }
                else if (TileType == TileTypes.START)
                {

                    if(maze.startTile != null)
                        maze.startTile.SetType(TileTypes.WALKABLE, maze.IsOffset(new Vector2Int(maze.startTile.x, maze.startTile.y)));

                    if (maze.endTile == tile)
                        maze.endTile = null;

                    tile.SetType(TileTypes.START, maze.IsOffset(new Vector2Int(tile.x, tile.y)));

                    maze.startTile = tile;
                }
                else if (TileType == TileTypes.END)
                {
                    if (maze.endTile != null)
                        maze.endTile.SetType(TileTypes.WALKABLE, maze.IsOffset(new Vector2Int(maze.startTile.x, maze.startTile.y)));

                    if (maze.startTile == tile)
                        maze.startTile = null;

                    tile.SetType(TileTypes.END, maze.IsOffset(new Vector2Int(tile.x, tile.y)));

                    maze.endTile= tile;
                }

            }
        }
        //test 
        if (Input.GetKeyDown(KeyCode.T))
        {
            maze.StartSearch(new BreadthFS());
        }
    }
}
