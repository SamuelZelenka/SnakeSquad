using UnityEngine;

public class HexNode : MonoBehaviour, IPoolable
{
    public Vector2Int coordinate;
    public bool isWall;

    [SerializeField] private Sprite _wallSprite;
    [SerializeField] private Sprite _hexSprite;

    private SpriteRenderer _spriteRender;

    private void OnEnable()
    {
        _spriteRender = GetComponent<SpriteRenderer>();
        Snake.onMoveTick += CheckVisibility;
    }

    private void OnDisable()
    {
        Snake.onMoveTick -= CheckVisibility;
    }

    public  void CheckVisibility(Vector2Int moveToCoordinate)
    { 
        float distanceToPlayer = Vector2.Distance(moveToCoordinate, coordinate);

       Color color = _spriteRender.color;


       float alpha = 1 - distanceToPlayer / GameBoard.RevealRange;
       alpha = GameBoard.GetFadeValue(alpha);

       color = new Color(color.r, color.g, color.b, alpha);

       _spriteRender.color = color;

        if (Application.isPlaying && distanceToPlayer > GameBoard.RevealRange)
        {
            GameBoard.ReleaseNode(this);
        }
    }
    public static bool IsCoordinateWall(Vector2Int testCoordinate)
    {
        const int MIN_PERCENTAGE = 1, MAX_PERCENTAGE = 101;
        bool isCoordinateTooCloseToSpawn = Vector2.Distance(testCoordinate, Vector2.zero) > GameBoard.SafeSpawnRange;
        if (isCoordinateTooCloseToSpawn)
        {
            return GameBoard.RandomNumberByCoordinate(testCoordinate, MIN_PERCENTAGE, MAX_PERCENTAGE) < GameBoard.WallSpawnChance;
        }
        return false;
    }

    public void UpdateNode(Vector2Int newCoordinate, NodeObject newValue)
    {
        coordinate = newCoordinate;
        transform.position = HexGrid.GetWorldPos(coordinate);
        isWall = IsCoordinateWall(coordinate);
        
        if (isWall)
        {
            SetWall();
        }
        else
        {
            SetHex();
        }
    }

    private void SetWall()
    {
        _spriteRender.sprite = _wallSprite;
    }
    private void SetHex()
    {
        _spriteRender.sprite = _hexSprite;
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
