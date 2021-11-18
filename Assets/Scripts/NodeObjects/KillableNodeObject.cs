using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableNodeObject : NodeObject
{
    public override void OnCollision(Vector2Int moveToCoordinate, Snake snake)
    {
        if (IsGameOver(moveToCoordinate))
        {
            GameSession.onGameOver?.Invoke();
        }
    }
    private bool IsGameOver(Vector2Int moveToCoordinate)
    {
        return moveToCoordinate == coordinate;
    }
}
