using UnityEngine;

public class HexNode : MonoBehaviour, IPoolable
{
    public NodeObject value;
    public Vector2Int coordinate;

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

    public  void CheckVisibility(Vector2Int moveToCoordinate, Squad squad)
    {
        float distanceToPlayer = Vector2.Distance(moveToCoordinate, coordinate);
        Color color = _spriteRender.color;
        color = new Color(color.r, color.g, color.b, 1 - distanceToPlayer/visibilityRange);
        _spriteRender.color = color;

        if (Application.isPlaying && distanceToPlayer > visibilityRange)
        {
            GameBoard.ReleaseNode(this);
        }
    }

    public void SetActive(bool active) => base.gameObject.SetActive(active);
}
