using System.Threading.Tasks;
using UnityEngine;

public class SnakeSegment : KillableNodeObject
{
    public SnakeSegment nextSegment;

    public async Task MoveToTarget(Vector2Int gridCoordinate, float moveSpeed)
    {
        Vector2Int previousCoordinate = coordinate;
        coordinate = gridCoordinate;
        
        GameBoard.RemoveNodeObjectAt(previousCoordinate);
        GameBoard.SetNodeObjectAt(coordinate, this);

        Vector2 worldPosition = HexGrid.GetWorldPos(coordinate);

        float pointValue = 0; 

        while (Application.isPlaying)
        {
            if (transform == null)
            {
                break;
            }
            if (Vector2.Distance(transform.position, worldPosition) > 0.001f)
            {
                pointValue += Time.deltaTime / moveSpeed;
            
                transform.position = Vector2.Lerp(transform.position, worldPosition, pointValue);
                 
            }
            await Task.Yield();
        }
    }
}