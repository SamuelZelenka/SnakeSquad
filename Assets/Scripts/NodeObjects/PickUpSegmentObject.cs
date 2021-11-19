using UnityEngine;

public class PickUpSegmentObject : NodeObject
{
    [SerializeField] private SnakeSegment snakeSegment;
    [SerializeField] private int score = 1;
    public override void OnCollision( Vector2Int moveToCoordinate, Snake snake)
    {
        if (moveToCoordinate == coordinate)
        {
            AddSquadMember(snake);
            GameSession.onScoreAdd?.Invoke(score);
        }
    }

    private void AddSquadMember(Snake snake)
    {
        SnakeSegment newMember = GameBoard.SpawnAt<SnakeSegment>(coordinate);

        snake.Add(newMember);

        GameBoard.RemoveNodeObjectAt(coordinate);
        GameBoard.SetNodeObjectAt(coordinate, newMember);

        Destroy(gameObject);
    }
}
