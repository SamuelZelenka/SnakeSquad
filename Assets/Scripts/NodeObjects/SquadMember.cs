using System.Threading.Tasks;
using UnityEngine;

public class SquadMember : NodeObject
{
    [SerializeField] public SquadMember nextSquadMember;
    [SerializeField] public SquadMember previousSquadMember;
    
    private void Start()
    {
        Squad.onMoveTick += OnCollision;
    }

    private void OnDisable()
    {
        Squad.onMoveTick -= OnCollision;
    }

    public override void OnCollision(Vector2Int moveToCoordinate, Squad squad)
    {
        if (IsGameOver(moveToCoordinate) && moveToCoordinate != squad.tail.coordinate)
        {
            Debug.Log("You F Up."); //Insert Game Over here
        }
    }

    private bool IsGameOver(Vector2Int moveToCoordinate)
    {
        return moveToCoordinate == coordinate;
    }

    public async Task MoveToTarget(Vector2Int gridCoordinate, float moveSpeed)
    {
        Vector2Int previousCoordinate = coordinate;
        coordinate = gridCoordinate;
        
        GameBoard.RemoveNodeObjectAt(previousCoordinate);
        GameBoard.SetNodeObjectAt(coordinate, this);

        Vector2 worldPosition = HexGrid.GetWorldPos(coordinate);

        float pointValue = 0; 

        while (Application.isPlaying && Vector2.Distance(transform.position, worldPosition) > 0.001f)
        {
            pointValue += Time.deltaTime / moveSpeed;
            
            transform.position = Vector2.Lerp(transform.position, worldPosition, pointValue);
            await Task.Yield();
        }
    }
}
