using System.Collections;
using UnityEngine;

public class Squad : MonoBehaviour
{
    [SerializeField, Range(0, 5)] private float _tickSpeedSeconds;
    [SerializeField, Range(0, 10)] private float _moveSpeed;
    [SerializeField] private GameObject _directionMarker;

    public delegate void MovementHandler(Vector2Int coordinate);
    public static MovementHandler onMoveTick;

    public HexDirection currentDirection;
    public Soldier head;
    public Soldier tail;

    private float MoveSpeed => _moveSpeed / 100;
    
    private void Start()
    {
        head = SpawnManager.SpawnAt<Soldier>(Vector2Int.zero).GetComponent<Soldier>();
        tail = head;
        Camera.main.transform.SetParent(head.transform);
        StartCoroutine(MoveTick());
        onMoveTick += UpdateMarker;
    }

    private void Update()
    {
        CheckInput();
    }

    private void UpdateMarker(Vector2Int coordinate)
    {
        _directionMarker.transform.position = transform.position + HexGrid.GetWorldPos(coordinate);
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
    }

    private void ChangeDirection(bool isClockwise)
    {
        const int CLOCKWISE = 1, COUNTER_CLOCKWISE = -1;
        currentDirection = IncrementDirection( isClockwise ? CLOCKWISE : COUNTER_CLOCKWISE );
        UpdateMarker(HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection));
    }

    private HexDirection IncrementDirection(int direction)
    {
        const int MAX_DIRECTION = (int)HexDirection.NE + 1;
        int newDirection = ((int)currentDirection + direction % MAX_DIRECTION + MAX_DIRECTION) % MAX_DIRECTION; // wraps the direction around in both direction.
        return (HexDirection)newDirection;
    }

    public void Add(Soldier soldier)
    {
        tail.nextSoldier = soldier;
        tail = soldier;
        soldier.nextSoldier = null;
        StartCoroutine(soldier.MoveToTarget(tail.coordinate, _moveSpeed));
    }
    
    private IEnumerator MoveTick()
    {
        yield return new WaitForSeconds(_tickSpeedSeconds);
        
        Vector2Int moveToCoordinate =  HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection);
        HexNode nextNode = GameBoard.GetNode(moveToCoordinate);
        if (nextNode.value != null)
        {
            Add(nextNode.value as Soldier);
        }
        
        onMoveTick?.Invoke(moveToCoordinate);
        StartCoroutine(head.MoveToTarget(moveToCoordinate, MoveSpeed));
        StartCoroutine(MoveTick());
    }
}
