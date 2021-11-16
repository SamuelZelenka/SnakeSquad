using UnityEngine;

public class HexNode : MonoBehaviour, IPoolable
{
    public NodeObject value;
    public Vector2Int coordinate;
    
    [SerializeField, Range(0, 100)] private int wallChance;
    [SerializeField] private Sprite wallSprite;
    [SerializeField] private Sprite hexSprite;

    private bool _isWall;
    private float visibilityRange = 4;
    private SpriteRenderer _spriteRender;

    private void OnEnable()
    {
        _spriteRender = GetComponent<SpriteRenderer>();
        Squad.onMoveTick += CheckVisibility;
    }

    private void OnDisable()
    {
        Squad.onMoveTick -= CheckVisibility;
    }

    public  void CheckVisibility(Vector2Int moveToCoordinate)
    {
        float distanceToPlayer = Vector2.Distance(moveToCoordinate, coordinate);
        if (!_isWall)
        {
           Color color = _spriteRender.color;
           color = new Color(color.r, color.g, color.b, 1 - distanceToPlayer/visibilityRange);
           _spriteRender.color = color;
        }

        if (Application.isPlaying && distanceToPlayer > visibilityRange)
        {
            GameBoard.ReleaseNode(this);
        }
    }

    public void CheckIfWall()
    {
        const int MAX_PERCECNTAGE = 100;
        int randomValue = HexGrid.RandomNumberByCoordinate(coordinate, 0, MAX_PERCECNTAGE);

        _isWall = randomValue < wallChance;
        if (_isWall)
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
        _spriteRender.sprite = wallSprite;
        _spriteRender.color = new Color(1, 1, 1, 1);
    }
    private void SetHex()
    {
        _spriteRender.sprite = hexSprite;
        _spriteRender.color = new Color(1, 1, 1, 0);
    }

    public void SetActive(bool active)
    {
        base.gameObject.SetActive(active);
    }
}
