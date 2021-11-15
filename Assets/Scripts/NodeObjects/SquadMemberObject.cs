using UnityEngine;

public class SquadMemberObject : NodeObject
{
    [SerializeField] private SquadMember squadMember;
    
    public override void OnCollision(Squad squad)
    {
        if (squad.head.coordinate == coordinate)
        {
            AddSquadMember(squad);
        }
    }

    private void AddSquadMember(Squad squad)
    {
            SquadMember newMember = PrefabSpawner.SpawnAt<SquadMember>(coordinate);

            squad.Add(newMember);

            GameBoard.RemoveNodeObjectAt(coordinate);
            GameBoard.SetNodeObjectAt(coordinate, newMember);

            Destroy(gameObject);
    }
}
