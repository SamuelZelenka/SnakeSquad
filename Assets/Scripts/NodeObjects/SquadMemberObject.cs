using UnityEngine;

public class SquadMemberObject : NodeObject
{
    [SerializeField] private SquadMember squadMember;
    
    public override void OnCollision(Squad squad)
    {
        AddSquadMember(squad);

        GameSession.OnInterract.Invoke(1);
    }

    private void AddSquadMember(Squad squad)
    {
        if (squad.head.coordinate == coordinate)
        {
            SquadMember newMember = PrefabSpawner.SpawnAt<SquadMember>(coordinate);

            squad.Add(newMember);

            GameBoard.RemoveNodeObjectAt(coordinate);
            GameBoard.SetNodeObjectAt(coordinate, newMember);

            Destroy(gameObject);
        }
    }
}
