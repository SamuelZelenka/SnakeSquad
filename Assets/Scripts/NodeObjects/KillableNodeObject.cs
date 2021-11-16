using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableNodeObject : NodeObject
{
    public override void OnCollision(Vector2Int moveToCoordinate, Squad squad)
    {
        if (IsGameOver(moveToCoordinate))
        {
            Debug.Log("You F Up."); //Insert Game Over here
        }
    }
    private bool IsGameOver(Vector2Int moveToCoordinate)
    {
        return moveToCoordinate == coordinate;
    }
}
