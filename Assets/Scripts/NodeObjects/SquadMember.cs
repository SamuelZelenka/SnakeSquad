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
        if (squad.head.coordinate == coordinate)
        {
            Debug.Log("You F Up.");
        }
    }

    public async Task MoveToTarget(Vector2Int gridCoordinate, float __moveSpeed)
    {
        Vector2Int previousCoordinate = coordinate;
        coordinate = gridCoordinate;
        
        GameBoard.RemoveNodeObjectAt(previousCoordinate);
        GameBoard.AddNodeObjectAt(coordinate, this);

        Vector2 worldPosition = HexGrid.GetWorldPos(coordinate);

        float t = 0; 

        while (Application.isPlaying && Vector2.Distance(transform.position, worldPosition) > 0.001f)
        {
            t += Time.deltaTime / __moveSpeed;
            
            transform.position = Vector2.Lerp(transform.position, worldPosition, t);
            await Task.Yield();
        }
    }
    
}
