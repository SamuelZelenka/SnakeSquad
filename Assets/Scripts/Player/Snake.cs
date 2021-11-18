using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public delegate void MovementHandler(Vector2Int coordinate);
    public static MovementHandler onMoveTick;
    
    public SnakeSegment head;
    public SnakeSegment tail;
        
    public HexDirection lastDirection;
    public HexDirection currentDirection;

    [SerializeField, Range(0, 5)] private float _tickRateSeconds;
    [SerializeField, Range(0, 50)] private float _moveSpeed;
    [SerializeField, Range(0, 5)] private int _revealRange;
    [SerializeField] private GameObject _directionMarker;

    private int TickRateMilliseconds => (int)(_tickRateSeconds * 1000);

    private Vector2Int CurrentDirectionVector => head.coordinate + HexGrid.GetDirectionVector(head.coordinate, currentDirection);

    private void Start()
    {
        GameSession.onGameReset += ResetSnake;
        ResetSnake();
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
        transform.SetParent(null);
        RemoveMembersFrom(head);
        head = GameBoard.SpawnAt<SnakeSegment>(Vector2Int.zero);   
        transform.position = HexGrid.GetWorldPos(Vector2Int.zero);
        tail = head;
        transform.SetParent(head.transform);
        UpdateMarker(CurrentDirectionVector);
        MoveTick();
    }

    public void RemoveMembersFrom(SnakeSegment startSegment)
    {
        if (startSegment != null)
        {
            if (startSegment.nextSegment != null)
            {
                RemoveMembersFrom(startSegment.nextSegment);
            }
            Destroy(startSegment.gameObject); 
        }
    }
    public void Add(SnakeSegment snakeSegment)
    {
        snakeSegment.nextSegment = head.nextSegment;

        head.nextSegment = snakeSegment;

        SnakeSegment currentMember = snakeSegment;

        if (head == tail)
        {
            tail = snakeSegment;
        }
        while (currentMember.nextSegment != null)
        {
            currentMember.coordinate = currentMember.nextSegment.coordinate;
            currentMember = currentMember.nextSegment;
        }
    }

    private async void MoveTick()
    {
        while (Application.isPlaying)
        {
            if (GameSession.isPlaying)
            {
                NodeObject moveToNodeObject = GameBoard.GetObjectAt(CurrentDirectionVector);
                onMoveTick?.Invoke(CurrentDirectionVector);

                if (moveToNodeObject != null)
                {
                    moveToNodeObject.OnCollision(CurrentDirectionVector, this);
                }
                else if (GameBoard.GetHexNode(CurrentDirectionVector).isWall)
                {
                    GameSession.onGameOver?.Invoke();
                }

                lastDirection = currentDirection;
                await Task.WhenAll(MoveAllMembers());
                await Task.Delay(TickRateMilliseconds);
            }
            else
            {
                break;
            }
        }

        Task[] MoveAllMembers()
        {
            Vector2Int moveToCoordinate = CurrentDirectionVector;
            SnakeSegment currentMember = head;
            List<Task> moveMemberTasks = new List<Task>();

            do
            {
                Vector2Int previousPos = currentMember.coordinate;
                Task moveMemberTask = currentMember.MoveToTarget(moveToCoordinate, _moveSpeed);

                moveMemberTasks.Add(moveMemberTask);
                moveToCoordinate = previousPos;
                currentMember = currentMember.nextSegment;

            } while (currentMember != null);

            return moveMemberTasks.ToArray();
        }
    }
}
