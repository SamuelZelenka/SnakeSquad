using UnityEngine;

public class NewMember : NodeObject
{
    [SerializeField] private SquadMember squadMember;
    
    public override void OnCollision( Vector2Int moveToCoordinate, Squad squad)
    {
        if (moveToCoordinate == coordinate)
        {
            AddSquadMember(squad);
        }
    }

    private void AddSquadMember(Squad squad)
    {
            SquadMember newMember = GameBoard.SpawnAt<SquadMember>(coordinate);

            squad.Add(newMember);

            GameBoard.RemoveNodeObjectAt(coordinate);
            GameBoard.SetNodeObjectAt(coordinate, newMember);

            Destroy(gameObject);
    }
}
