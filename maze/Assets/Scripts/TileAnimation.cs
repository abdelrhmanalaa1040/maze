using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TileAnimation : MonoBehaviour
{
    public AnimationCurve curve;
    public Tile tile;
    [Range(0, 1)]
    public float delay;
    public float AnimationSpeed;
    Vector2 _tileSize;
    float _time;

    void Start()
    {
        _tileSize = transform.localScale;
        transform.localScale = Vector3.one * 0.01f;
    }
    void Update()
    {
        float totalTime = (_time / AnimationSpeed) - (delay * (tile.x + tile.y));
        _time += Time.deltaTime;
        Vector2 currentTileSize = _tileSize * curve.Evaluate(totalTime);
        transform.localScale = currentTileSize;
        
        if(totalTime > curve.length)
        {
            Destroy(gameObject.GetComponent<TileAnimation>());
        }
    }

}
