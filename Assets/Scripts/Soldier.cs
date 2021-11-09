using System.Collections;
using UnityEngine;

public class Soldier : NodeObject
{
    [SerializeField] public Soldier nextSoldier;
    [SerializeField] public Soldier previousSoldier;

    public Vector2Int coordinate;

    public override NodeObject OnCollision(Squad collidingSquad)
    {
        collidingSquad.Add(this);
        StartCoroutine(MoveToTarget(collidingSquad.tail.coordinate, 2));
        return this;
    }
    
    public override GameObject GetGameObject() => gameObject;

    public IEnumerator MoveToTarget(Vector2Int gridCoordinate, float speed)
    {
        Vector2Int previousCoordinate = coordinate;
        coordinate = gridCoordinate;
        GameBoard.GetNode(gridCoordinate).value = this;
        GameBoard.GetNode(previousCoordinate).value = null;
        Vector2 worldPosition = HexGrid.GetWorldPos(coordinate);
        
        while (Application.isPlaying && Vector2.Distance(transform.position, worldPosition) > 0.001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldPosition, speed);
            yield return null;
        }
    }
}
