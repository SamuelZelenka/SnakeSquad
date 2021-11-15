using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class SquadMember : NodeObject
{
    [SerializeField] public SquadMember nextSquadMember;
    [SerializeField] public SquadMember previousSquadMember;

    private Squad _squad;
    private void Start()
    {
        Squad.onMoveTick += OnCollision;
    }

    private void OnDisable()
    {
        Squad.onMoveTick -= OnCollision;
    }

    public override void OnCollision(Squad squad)
    {
        if (IsGameOver(squad))
        {
            Debug.Log("You F Up.");
        }
    }

    private bool IsGameOver(Squad squad)
    {
        return squad.head.coordinate == coordinate && 
            GameBoard.GetNodeObject(squad.head.coordinate) != squad.head;
    }


    public async Task MoveToTarget(Vector2Int gridCoordinate, float __moveSpeed)
    {
        Vector2Int previousCoordinate = coordinate;
        coordinate = gridCoordinate;
        
        GameBoard.RemoveNodeObjectAt(previousCoordinate);
        GameBoard.SetNodeObjectAt(coordinate, this);

        Vector2 worldPosition = HexGrid.GetWorldPos(coordinate);

        float pointValue = 0; 

        while (Application.isPlaying && Vector2.Distance(transform.position, worldPosition) > 0.001f)
        {
            pointValue += Time.deltaTime / __moveSpeed;
            
            transform.position = Vector2.Lerp(transform.position, worldPosition, pointValue);
            await Task.Yield();
        }
    }
    
}
