using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Squad : MonoBehaviour
{
    [SerializeField, Range(0, 5)] private float _tickRateSeconds;
    [SerializeField, Range(0, 50)] private float _moveSpeed;

    [SerializeField] private GameObject _directionMarker;

    public delegate void MovementHandler(Squad squad);
    public static MovementHandler onMoveTick;

    public HexDirection currentDirection;
    public SquadMember head;
    public SquadMember tail;

    private int tickRateMilliseconds => (int)(_tickRateSeconds * 1000);

    private void Start()
    {
        head = PrefabSpawner.SpawnAt<SquadMember>(Vector2Int.zero);
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
            IncrementDirection(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            IncrementDirection(false);
        } 
        UpdateMarker(HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection));
    }

    private void IncrementDirection(bool isClockwise)
    {
        const int CLOCKWISE = 1, COUNTER_CLOCKWISE = -1, MAX_DIRECTION = (int)HexDirection.NE + 1;

        int newDirection = isClockwise ? CLOCKWISE : COUNTER_CLOCKWISE;

        int unwrappedDirection = (int)(currentDirection + newDirection % MAX_DIRECTION + MAX_DIRECTION);

        int wrappedDirection = unwrappedDirection % MAX_DIRECTION;

        currentDirection = (HexDirection) wrappedDirection;
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

    private async void MoveTick()
    {
        Vector2Int moveToCoordinate;

        while (true)
        {
            moveToCoordinate = HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection);
            NodeObject moveToNodeObject = GameBoard.GetNodeObject(moveToCoordinate);
            if (moveToNodeObject)
            {
                moveToNodeObject.OnCollision(this);
            }
            onMoveTick?.Invoke(this);
            await Task.WhenAll(MoveAllMembers());
            await Task.Delay(tickRateMilliseconds);
        }

        Task[] MoveAllMembers()
        {
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
