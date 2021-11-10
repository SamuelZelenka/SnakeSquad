using System.Collections;
using UnityEngine;

public class Soldier : NodeObject
{
    [SerializeField] public Soldier nextSoldier;
    [SerializeField] public Soldier previousSoldier;

    public Vector2Int coordinate;
    public Squad squad;
    
    public override NodeObject OnCollision<T>(T other)
    {
        Soldier soldier = other as Soldier;
        if (soldier != null)
        {
            squad = soldier.squad;
            squad.Add(this);
            StartCoroutine(MoveToTarget(squad.head.coordinate, 2));
        }
        return this;
    }
    
    public override GameObject GetGameObject() => gameObject;

    public IEnumerator MoveToTarget(Vector2Int gridCoordinate, float speed)
    {
        Vector2Int previousCoordinate = coordinate;
        coordinate = gridCoordinate;
        
        Vector2 worldPosition = HexGrid.GetWorldPos(coordinate);
        
        while (Application.isPlaying && Vector2.Distance(transform.position, worldPosition) > 0.001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldPosition, speed);
            yield return null;
        }
        if (nextSoldier != null)
        {
            yield return StartCoroutine(nextSoldier.MoveToTarget(previousCoordinate, speed));
        }
    }
}
