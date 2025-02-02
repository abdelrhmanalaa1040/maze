using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MazeData
{
    public int _width, _height;
    public float tileSize;
    public List<TileData> tileData;
}

[Serializable]
public class TileData
{
    public Color color;
    public TileTypes tileType;
    public int tileCost;
}
