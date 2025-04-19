using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
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
        if (AnimationSpeed > 0)
        {
            _tileSize = transform.localScale;
            transform.localScale = Vector3.one * 0.01f;
        }
    }

    void Update()
    {
        if(AnimationSpeed > 0)
        {
            float totalTime = (_time / AnimationSpeed) - (delay * (tile.x + tile.y));
            _time += Time.deltaTime;
            Vector2 currentTileSize = _tileSize * curve.Evaluate(totalTime);
            transform.localScale = currentTileSize;

            if (totalTime > curve.length)
            {
                Destroy(gameObject.GetComponent<TileAnimation>());
            }
        }
    }
}
