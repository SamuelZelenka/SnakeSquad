using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Squad : MonoBehaviour
{
    [SerializeField, Range(0, 5)] private float _tickRate;
    [SerializeField, Range(0, 50)] private float _moveSpeed;

    [SerializeField] private GameObject _directionMarker;

    public delegate void MovementHandler(Squad squad);
    public static MovementHandler onMoveTick;

    public HexDirection currentDirection;
    public SquadMember head;
    public SquadMember tail;

    private void Start()
    {
        head = SpawnManager.SpawnAt<SquadMember>(Vector2Int.zero);
        tail = head;
        transform.SetParent(head.transform);
        UpdateMarker(HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection));
        MoveTick();
    }

    private void Update()
    {
        CheckInput();
    }

    private void UpdateMarker(Vector2Int coordinate)
    {
        _directionMarker.transform.position = HexGrid.GetWorldPos(coordinate);
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeDirection(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeDirection(false);
        } 
        UpdateMarker(HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection));
    }

    private void ChangeDirection(bool isClockwise)
    {
        const int CLOCKWISE = 1, COUNTER_CLOCKWISE = -1;
        currentDirection = IncrementDirection( isClockwise ? CLOCKWISE : COUNTER_CLOCKWISE );
    }

    private HexDirection IncrementDirection(int direction)
    {
        const int MAX_DIRECTION = (int)HexDirection.NE + 1;
        int newDirection = ((int)currentDirection + direction % MAX_DIRECTION + MAX_DIRECTION) % MAX_DIRECTION; // wraps the direction around in both direction.
        return (HexDirection)newDirection;
    }

    public void Add(SquadMember squadMember)
    {
        squadMember.nextSquadMember = head.nextSquadMember;
        head.nextSquadMember = squadMember;
        SquadMember currentMember = squadMember;
        while (currentMember.nextSquadMember != null)
        {
            currentMember.coordinate = currentMember.nextSquadMember.coordinate;
            currentMember = currentMember.nextSquadMember;
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        throw new NotImplementedException();
    }

    private async void MoveTick()
    {
        while (true)
        {
            Vector2Int moveToCoordinate =  HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection);
            SquadMember currentMember = head;
            List<Task> moveMemberTasks = new List<Task>();
            
            onMoveTick?.Invoke(this);
            
            do
            {
                Vector2Int previousPos = currentMember.coordinate;
                Task moveMemberTask = currentMember.MoveToTarget(moveToCoordinate, _moveSpeed);
                
                moveMemberTasks.Add(moveMemberTask);
                moveToCoordinate = previousPos;
                currentMember = currentMember.nextSquadMember;
            } while (currentMember != null);
            
            await Task.WhenAll(moveMemberTasks);
            await Task.Delay((int)(_tickRate * 1000));
        }
    }
}
