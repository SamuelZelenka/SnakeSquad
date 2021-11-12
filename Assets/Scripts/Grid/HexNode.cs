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

    private void OnDisable()
    {
        Squad.onMoveTick -= CheckVisibility;
    }

    public  void CheckVisibility(Squad squad)
    {
        float distanceToPlayer = Vector2.Distance(squad.head.coordinate, coordinate);
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1 - distanceToPlayer/visibilityRange);
        if (Application.isPlaying && distanceToPlayer > visibilityRange)
        {
            GameBoard.ReleaseNode(this);
        }
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

    public GameObject GameObject => gameObject;
}
