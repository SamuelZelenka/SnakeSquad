using UnityEngine;
using UnityEngine.UI;


public class InGameUI : MonoBehaviour
{
    [SerializeField] private Image screenGlow;
    private void Start()
    {
        Squad.onMoveTick += ScreenGlowEffect;
    }

    private void ScreenGlowEffect(Vector2Int coordinate)
    {
        Color color = screenGlow.color;
        screenGlow.color = new Color(color.r, color.g, color.b, Random.value);
    }

    public void RestartGame()
    {
    }
}
