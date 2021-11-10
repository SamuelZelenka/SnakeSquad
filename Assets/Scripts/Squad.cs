using System.Collections;
using UnityEngine;

public class Squad : MonoBehaviour
{
    [SerializeField, Range(0, 5)] private float _tickRate;
    [SerializeField, Range(1, 50)] private float _moveSpeed;
    
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
        head.squad = this;
        transform.SetParent(head.transform);
        UpdateMarker(HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection));
        StartCoroutine(MoveTick());
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
    }
    
    private IEnumerator MoveTick()
    {
        Vector2Int moveToCoordinate =  HexGrid.GetCoordinateInDirection(head.coordinate, currentDirection);
        onMoveTick?.Invoke(moveToCoordinate);
        
        yield return new WaitForSeconds(_tickRate);

        if (CollisionCheck(moveToCoordinate, out HexNode checkedNode))
        {
            checkedNode.value.OnCollision(head);
            checkedNode.value = null;
        }

        yield return StartCoroutine(head.MoveToTarget(moveToCoordinate, MoveSpeed));
        
        StartCoroutine(MoveTick());
        
    }

    private bool CollisionCheck(Vector2Int coordinate, out HexNode checkedNode)
    {
        checkedNode = GameBoard.GetNode(coordinate);
        return checkedNode && checkedNode.value != null;
    }
}
