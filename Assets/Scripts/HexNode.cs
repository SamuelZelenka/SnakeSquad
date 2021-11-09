using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexNode : MonoBehaviour, IPoolable
{
    public NodeObject value;
    public Vector2Int coordinate;
    
    private void OnBecameInvisible()
    {
        if (Application.isPlaying && value == null)
        {
            GameBoard.ReleaseNode(this);
        }
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

    public GameObject GameObject => gameObject;
}
