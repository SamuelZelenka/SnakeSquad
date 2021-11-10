using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexNode : MonoBehaviour, IPoolable
{
    public NodeObject value;
    public Vector2Int coordinate;

    private float visibilityRange = 4;
    private SpriteRenderer spriteRender;

    private void OnEnable()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        Squad.onMoveTick += CheckVisibility;
    }

    public  void CheckVisibility(Vector2Int headCoordinate)
    {
        float distanceToPlayer = Vector2.Distance(headCoordinate, coordinate);
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1 - distanceToPlayer/visibilityRange);
        if (Application.isPlaying && distanceToPlayer > visibilityRange)
        {
            Squad.onMoveTick -= CheckVisibility;
            GameBoard.ReleaseNode(this);
        }
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

    public GameObject GameObject => gameObject;
}
