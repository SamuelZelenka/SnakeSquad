using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Squad : MonoBehaviour
{
    
    
    public delegate void MovementHandler(Vector2Int coordinate, Squad squad);
    public static MovementHandler onMoveTick;
    
    public SquadMember head;
    public SquadMember tail;
        
    public HexDirection lastDirection;
    public HexDirection currentDirection;

    [SerializeField, Range(0, 5)] private float _tickRateSeconds;
    [SerializeField, Range(0, 50)] private float _moveSpeed;
    [SerializeField] private GameObject _directionMarker;

    private int TickRateMilliseconds => (int)(_tickRateSeconds * 1000);

    private Vector2Int CurrentDirectionVector => head.coordinate + HexGrid.GetDirectionVector(head.coordinate, currentDirection);

    private void Start()
    {
        head = GameBoard.SpawnAt<SquadMember>(Vector2Int.zero);
        tail = head;
        transform.SetParent(head.transform);
        UpdateMarker(CurrentDirectionVector);
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
            IncrementDirection(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            IncrementDirection(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RemoveMember(head.nextSquadMember);
        }
        UpdateMarker(CurrentDirectionVector);
    }

    public void IncrementDirection(bool isClockwise)
    {
        const int CLOCKWISE = 1, COUNTER_CLOCKWISE = -1, MAX_DIRECTION = (int)HexDirection.NE + 1;

        int newDirection = isClockwise ? CLOCKWISE : COUNTER_CLOCKWISE;

        HexDirection wrappedDirection = (HexDirection)(((int)currentDirection + newDirection % MAX_DIRECTION + MAX_DIRECTION) % MAX_DIRECTION);

        HexDirection oppositeOfLastDirection = (HexDirection)(((int)lastDirection + MAX_DIRECTION / 2) % MAX_DIRECTION);

        currentDirection = wrappedDirection == oppositeOfLastDirection ? currentDirection : (HexDirection) wrappedDirection;
    }

    public void ResetSnake()
    {
        SquadMember currentMember = head;

    }

    public void RemoveMember(SquadMember member)
    {
        if (member.nextSquadMember == null && member.previousSquadMember == null)
        {
            //Game Over
        }
        else if (member == head)
        {
            head = member.nextSquadMember;
        }
        else if (member == tail)
        {
            tail = member.previousSquadMember;
        }

        member.previousSquadMember.nextSquadMember = member.nextSquadMember;
        member.nextSquadMember.previousSquadMember = member.previousSquadMember;
        Destroy(member.gameObject);
    }
    
    public void Add(SquadMember squadMember)
    {
        squadMember.nextSquadMember = head.nextSquadMember;
        squadMember.previousSquadMember = head;
        
        head.nextSquadMember = squadMember;

        SquadMember currentMember = squadMember;

        if (head == tail)
        {
            tail = squadMember;
        }
        while (currentMember.nextSquadMember != null)
        {
            currentMember.coordinate = currentMember.nextSquadMember.coordinate;
            currentMember = currentMember.nextSquadMember;
        }
    }

    private async void MoveTick()
    {
        while (Application.isPlaying)
        {

            NodeObject moveToNodeObject = GameBoard.GetNodeObject(CurrentDirectionVector);
            onMoveTick?.Invoke(CurrentDirectionVector, this);

            if (moveToNodeObject != null)
            {
                moveToNodeObject.OnCollision(CurrentDirectionVector, this);
            }

            lastDirection = currentDirection;
            await Task.WhenAll(MoveAllMembers());
            await Task.Delay(TickRateMilliseconds);
        }
        
        Task[] MoveAllMembers()
        {
            Vector2Int moveToCoordinate = CurrentDirectionVector;
            SquadMember currentMember = head;
            List<Task> moveMemberTasks = new List<Task>();

            do
            {
                Vector2Int previousPos = currentMember.coordinate;
                Task moveMemberTask = currentMember.MoveToTarget(moveToCoordinate, _moveSpeed);

                moveMemberTasks.Add(moveMemberTask);
                moveToCoordinate = previousPos;
                currentMember = currentMember.nextSquadMember;

            } while (currentMember != null);

            return moveMemberTasks.ToArray();
        }
    }
}
