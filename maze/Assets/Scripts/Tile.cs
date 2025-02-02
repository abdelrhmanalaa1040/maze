using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileTypes
{
    WALKABLE,
    WALL,
    START,
    END
}

public class Tile : MonoBehaviour, IState
{

    [SerializeField] private Color _baseColor, _offsetColor, _wallColor, _pointColor, _visitedColor, _pathColor;

    [SerializeField] public SpriteRenderer _render;

    public int x, y;
    public string Name { get; set; }

    public TileTypes TileType;

    public void InitState(int x, int y)
    {
        Name = $"Tile {x} {y}";
        this.x = x;
        this.y = y;
    }

    public void Init(bool isOffset)
    {
        _render.color = isOffset ? _baseColor : _offsetColor;
    }

    public void SetType(TileTypes type, bool isOffset = false)
    {
        TileType = type;
        if(type == TileTypes.START || type == TileTypes.END)
        {
            _render.color = _pointColor;
        }
        else if (type == TileTypes.WALL)
        {
            _render.color = _wallColor;
        } 
        else if (type == TileTypes.WALKABLE && isOffset)
        {
            _render.color = _offsetColor;
        } else
        {
            _render.color = _baseColor;
        }
    }


    public void HighlightAsVisited()
    {
        _render.color = _visitedColor;
    }

    public void HighlightAsPath()
    {
        _render.color = _pathColor;
    }
}
