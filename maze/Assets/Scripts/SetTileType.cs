using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SetTileType : MonoBehaviour
{
    public TileTypes TileType;
    public Maze maze;

    private void Start()
    {
        maze = GameObject.FindFirstObjectByType<Maze>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider != null && hit.collider.gameObject.tag == "tile")
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                var isOffset = (tile.x % 2 == 0 && tile.y % 2 != 0) || (tile.x % 2 != 0 && tile.y % 2 == 0);

                if (TileType == TileTypes.WALL)
                {
                    tile.SetType(TileTypes.WALL, false);
                }
                else if (TileType == TileTypes.WALKABLE)
                {
                    tile.SetType(TileTypes.WALKABLE, isOffset);
                }
                else if (TileType == TileTypes.START)
                {
                    maze._tiles[maze.startTilePosition.x][maze.startTilePosition.y].SetType(TileTypes.WALKABLE, isOffset);
                }
            }
        }
    }
}
