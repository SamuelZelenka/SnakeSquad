using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PickUpSquadMember : NodeObject
{
    private void Start()
    {
        Squad.onMoveTick += OnCollision;
    }

    private void OnDisable()
    {
        Squad.onMoveTick -= OnCollision;
    }

    [SerializeField] private SquadMember squadMember;
    
    public override void OnCollision(Squad squad)
    {
        if (squad.head.coordinate == coordinate)
        {
            squad.Add(SpawnManager.SpawnAt<SquadMember>(coordinate));
            GameBoard.RemoveNodeObjectAt(coordinate);
            Destroy(gameObject);
        }
    }
}
