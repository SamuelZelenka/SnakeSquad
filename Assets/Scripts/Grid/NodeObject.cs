using UnityEngine;

public abstract class NodeObject : MonoBehaviour
{
    public Vector2Int coordinate;
    public abstract void OnCollision(Squad squad);

    public void SetCoordinate(Vector2Int newCoordinate)
    {
        coordinate = newCoordinate;
    }
}
